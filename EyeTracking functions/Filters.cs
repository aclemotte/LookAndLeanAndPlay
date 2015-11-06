using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tobii.Gaze.Core;

namespace LookAndPlayForm
{
    class Filters
    {
        Queue<double> GazeBufferX;
        Queue<double> GazeBufferY;

        public int FilterBufferSize;
        public filtertype filtertypeSelected = filtertype.meanMedian;
        public double thresholdGazeJumpNormalized;

        
        PointD lastFilterReturn;





        public Filters()
        {
            FilterBufferSize = 41;// settings.filterBufferSize;
            filtertypeSelected = filtertype.meanMedian;
            thresholdGazeJumpNormalized = 0.02;

            GazeBufferX = new Queue<double>(FilterBufferSize);
            GazeBufferY = new Queue<double>(FilterBufferSize);
        }

        /// <summary>
        /// Filtra segun lo seleccionado en filtertypeSelected si el argumento no es NAN
        /// </summary>
        /// <param name="posx"></param>
        /// <param name="posy"></param>
        /// <returns></returns>
        public PointD filterGazeData(PointD gazeData)
        {            
            if (gazeData.IsNaN())
                return gazeData;//q esta inicializado en NaN's
            else
            {
                PointD gazeDataFiltered = new PointD(double.NaN, double.NaN);

                if (filtertypeSelected == filtertype.average)
                    gazeDataFiltered = new PointD(getMovingAverageGaze(gazeData));
                if (filtertypeSelected == filtertype.meanMedian)
                    gazeDataFiltered = new PointD(getMeanMedianGazeFiltered(gazeData));

                return gazeDataFiltered;
            }            
        }

        public bool clearBuffers()
        {
            GazeBufferX.Clear();
            GazeBufferY.Clear();
            return true;
        }






        /// <summary>
        ///1. si el argumento no tiene NANs
        ///             4. se mete un elemento de la cola
        ///             5. se transforma en lista la cola y se ordena de mayor a menor
        ///             6. se promedia los puntos del medio de la lista y se retorna este valor
        ///         7. si el buffer no esta lleno se encola el argumento y se retorna el argumento
        ///9. si el argumento tiene NANs se retorna el argumento        
        /// </summary>
        /// <param name="GazePoint"></param>
        /// <returns></returns>
        PointD getMeanMedianGazeFiltered(PointD GazePoint)
        {
            lastFilterReturn = GazePoint;

            //1.si el argumento no tiene NANs
            if(nonNanGazeValues(GazePoint))
            {
                if (gazeJump(GazePoint))
                {
                    add2Buffer(GazePoint);

                    //5.se transforma en lista la cola y se ordena de mayor a menor
                    var listaTempX = GazeBufferX.ToList();
                    var listaTempY = GazeBufferY.ToList();
                    listaTempX.Sort();
                    listaTempY.Sort();

                    //deben existir al menos tres puntos en el buffer para hacer lo siguiente
                    if (GazeBufferX.Count > 2)
                    {
                        //si es impar se promedia el del medio con sus vecinos
                        //si es par el promedio de los 4 centrales

                        if (GazeBufferX.Count % 2 == 0)
                        {
                            // GazeBufferX.Count is even, need to get the middle two elements, add them together, then divide by 2
                            lastFilterReturn.X = (listaTempX[(GazeBufferX.Count / 2) - 2] + listaTempX[(GazeBufferX.Count / 2) - 1] +
                                                listaTempX[(GazeBufferX.Count / 2) + 0] + listaTempX[(GazeBufferX.Count / 2) + 1]) / 4;
                            lastFilterReturn.Y = (listaTempY[(GazeBufferX.Count / 2) - 2] + listaTempY[(GazeBufferX.Count / 2) - 1] +
                                                listaTempY[(GazeBufferX.Count / 2) + 0] + listaTempY[(GazeBufferX.Count / 2) + 1]) / 4;
                        }
                        else
                        {
                            // GazeBufferX.Count is odd, simply get the middle element
                            lastFilterReturn.X = (listaTempX[GazeBufferX.Count / 2 - 1] + listaTempX[(GazeBufferX.Count / 2) + 0] + listaTempX[(GazeBufferX.Count / 2) + 1]) / 3;
                            lastFilterReturn.Y = (listaTempY[GazeBufferX.Count / 2 - 1] + listaTempY[(GazeBufferX.Count / 2) + 0] + listaTempY[(GazeBufferX.Count / 2) + 1]) / 3;
                        }

                        return lastFilterReturn;
                    }
                    else//si el buffer tiene menos de 3 elementos
                    {
                        return GazePoint;
                    }
                }
                else //si se detecta un sacadico, se limpia el buffer y se retorna el argumento
                {
                    clearBuffers();
                    return GazePoint;
                }
            }//9.si el argumento tiene NANs se retorna el argumento
            else
                return GazePoint;
        }

        /// <summary>
        ///1.si el argumento no tiene NANs
        ///         4.1 mete argumento en la cola
        ///         4.2 se transforma en lista
        ///         4.3 se hace el promedio de la lista
        ///         4.4 se retorna el promedio de la lista
        /// 6. si el argumento tiene NANs
        ///     retorna argumento
        /// </summary>
        /// <param name="GazePoint"></param>
        /// <returns></returns>
        PointD getMovingAverageGaze(PointD GazePoint)
        {
            //1 si el argumento tiene NANs
            if (nonNanGazeValues(GazePoint))
            {
                add2Buffer(GazePoint);

                //4.2
                var listaTempX = GazeBufferX.ToList();
                var listaTempY = GazeBufferY.ToList();
                //4.3
                PointD AveragePoint = new PointD();
                AveragePoint.X = listaTempX.Average();
                AveragePoint.Y = listaTempY.Average();
                //4.4
                return AveragePoint;
            }//sino 6
            else
            {
                return GazePoint;
            }
        }








        /// <summary>
        /// se ve si el nuevo gaze es un sacadico
        /// para esto se mide la distancia entre: el nuevo gaze y la salida anterior del filtro
        /// si la distancia esta por encima de un umbral es un sacadico
        /// </summary>
        /// <param name="GazePoint"></param>
        /// <returns></returns>
        private bool gazeJump(PointD GazePoint)
        {
            if (PointD.distance(GazePoint, lastFilterReturn) > thresholdGazeJumpNormalized)
                return true;
            else
                return false;
        }   

        bool add2Buffer(PointD GazePoint)
        {
            if (bufferFull())
            {
                GazeBufferX.Dequeue();
                GazeBufferY.Dequeue();
            }

            GazeBufferX.Enqueue(GazePoint.X);
            GazeBufferY.Enqueue(GazePoint.Y);

            return true;
        }

        bool nonNanGazeValues(PointD GazePoint)
        {
            if (!double.IsNaN(GazePoint.X) && !double.IsNaN(GazePoint.Y))
                return true;
            else
                return false;
        }

        bool bufferFull()
        {
            if (GazeBufferX.Count == FilterBufferSize || GazeBufferY.Count == FilterBufferSize)
                return true;
            else
                return false;
        }

    }
}
