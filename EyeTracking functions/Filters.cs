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

        Queue<double> CursorBufferX;
        Queue<double> CursorBufferY;

        public int gazeBufferSize;
        public int cursorBufferSize;
        public filtertype filterTypeSelected = filtertype.meanMedian;
        public double CursorJumpThresholdNormalized;

        
        PointD lastFilterReturn;





        public Filters()
        {
            gazeBufferSize = 41;// settings.filterBufferSize;
            cursorBufferSize = 3;
            filterTypeSelected = filtertype.meanMedian;
            CursorJumpThresholdNormalized = 0.03;

            GazeBufferX = new Queue<double>(gazeBufferSize);
            GazeBufferY = new Queue<double>(gazeBufferSize);

            CursorBufferX = new Queue<double>(cursorBufferSize);
            CursorBufferY = new Queue<double>(cursorBufferSize);

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

                if (filterTypeSelected == filtertype.average)
                    gazeDataFiltered = new PointD(getMovingAverageGaze(gazeData));
                if (filterTypeSelected == filtertype.meanMedian)
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
            //1.si el argumento no tiene NANs
            if (nonNanGazeValues(GazePoint))
            {
                if (!gazeJump1(GazePoint))
                {
                    addPointD2Buffer(GazePoint, GazeBufferX, GazeBufferY, gazeBufferSize);

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
                        //return lastFilterReturn
                    }
                    else//si el buffer tiene menos de 3 elementos, se actualiza lastFilterReturn
                    {
                        lastFilterReturn = GazePoint;
                        //return lastFilterReturn
                    }

                    return lastFilterReturn;
                }
                else //si se detecta un sacadico, se limpia el buffer, se actualiza lastFilterReturn y se retorna el argumento
                {
                    lastFilterReturn = GazePoint;
                    clearBuffers();
                    return GazePoint;
                }
            }//9.si el argumento tiene NANs se retorna el argumento. no se actualiza lastFilterReturn
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
                addPointD2Buffer(GazePoint, CursorBufferX, CursorBufferY, cursorBufferSize);

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
        /// Busca si GazePoint está a una distancia mayor del CursorJumpThresholdNormalized
        /// para esto se mide la distancia entre: el nuevo gaze y la salida anterior del filtro
        /// si la distancia esta por encima de un umbral es un sacadico
        /// </summary>
        /// <param name="GazePoint"></param>
        /// <returns></returns>
        private bool gazeJump1(PointD GazePoint)
        {
            if (PointD.distance(GazePoint, lastFilterReturn) > CursorJumpThresholdNormalized)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Busca si GazePoint y los puntos del CursorBuffer se encuentran dentro de un área definida
        /// para esto se mide los mínimos y máximos sobre cada eje
        /// con los máximos y mínimos de cada eje se mide la diferencia
        /// si las diferencias estan por encima de un umbral es un sacadico
        /// </summary>
        /// <param name="GazePoint"></param>
        /// <returns></returns>
        private bool gazeJump2(PointD GazePoint)
        {
            return true;            
        }   


        bool addPointD2Buffer(PointD GazePoint, Queue<double> bufferX, Queue<double> bufferY, int bufferSize)
        {
            if (bufferFull(bufferX, bufferSize))
            {
                bufferX.Dequeue();
                bufferY.Dequeue();
            }

            bufferX.Enqueue(GazePoint.X);
            bufferY.Enqueue(GazePoint.Y);

            return true;
        }

        
        bool bufferFull(Queue<double> buffer, int bufferSize)
        {
            if (buffer.Count == bufferSize || buffer.Count == bufferSize)
                return true;
            else
                return false;
        }

        bool nonNanGazeValues(PointD GazePoint)
        {
            if (!double.IsNaN(GazePoint.X) && !double.IsNaN(GazePoint.Y))
                return true;
            else
                return false;
        }

    }
}
