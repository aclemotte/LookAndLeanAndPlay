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


        public int gazeBufferSize;
        public int cursorBufferSize;
        public filtertype filterTypeSelected = filtertype.average;
        public double CursorJumpThresholdNormalized;

        
        PointD lastFilterReturn;
        Queue<double> GazeBufferX;
        Queue<double> GazeBufferY;
        //Queue<double> CursorBufferX;
        //Queue<double> CursorBufferY;





        public Filters()
        {
            gazeBufferSize = 20;// settings.filterBufferSize;
            //cursorBufferSize = 1;
            filterTypeSelected = filtertype.average;
            CursorJumpThresholdNormalized = 0.03;

            GazeBufferX = new Queue<double>(gazeBufferSize);
            GazeBufferY = new Queue<double>(gazeBufferSize);
            //CursorBufferX = new Queue<double>(cursorBufferSize);
            //CursorBufferY = new Queue<double>(cursorBufferSize);

        }

        /// <summary>
        /// Filtra segun lo seleccionado en filtertypeSelected si el argumento no es NAN
        /// </summary>
        /// <param name="posx"></param>
        /// <param name="posy"></param>
        /// <returns></returns>
        public PointD filterGazeData(PointD GazePoint)
        {            
            if (GazePoint.IsNaN())
                return GazePoint;
            else
            {
                //si es una fijacion se filtra y se actualiza lastFilterReturn 
                if (gazeFix1(GazePoint))
                {
                    if (filterTypeSelected == filtertype.average)
                        lastFilterReturn = new PointD(getMovingAverageGaze(GazePoint));
                    if (filterTypeSelected == filtertype.meanMedian)
                        lastFilterReturn = new PointD(getMeanMedianGazeFiltered(GazePoint));

                    //addPointD2Buffer(lastFilterReturn, CursorBufferX, CursorBufferY, cursorBufferSize);

                    return lastFilterReturn;

                }
                else//si no es una fijacion, se limpia el buffer GazeBufferX, se actualiza lastFilterReturn y se retorna el argumento
                {
                    lastFilterReturn = GazePoint;
                    //addPointD2Buffer(lastFilterReturn, CursorBufferX, CursorBufferY, cursorBufferSize);

                    clearBuffers();
                    return GazePoint;
                }
            }            
        }

        public bool clearBuffers()
        {
            GazeBufferX.Clear();
            GazeBufferY.Clear();
            return true;
        }






        PointD getMeanMedianGazeFiltered(PointD GazePoint)
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
                PointD gazeFiltered = new PointD();

                //si es impar se promedia el del medio con sus vecinos
                //si es par el promedio de los 4 centrales

                if (GazeBufferX.Count % 2 == 0)
                {
                    // GazeBufferX.Count is even, need to get the middle two elements, add them together, then divide by 2
                    gazeFiltered.X = (listaTempX[(GazeBufferX.Count / 2) - 2] + listaTempX[(GazeBufferX.Count / 2) - 1] +
                                        listaTempX[(GazeBufferX.Count / 2) + 0] + listaTempX[(GazeBufferX.Count / 2) + 1]) / 4;
                    gazeFiltered.Y = (listaTempY[(GazeBufferX.Count / 2) - 2] + listaTempY[(GazeBufferX.Count / 2) - 1] +
                                        listaTempY[(GazeBufferX.Count / 2) + 0] + listaTempY[(GazeBufferX.Count / 2) + 1]) / 4;
                }
                else
                {
                    // GazeBufferX.Count is odd, simply get the middle element
                    gazeFiltered.X = (listaTempX[GazeBufferX.Count / 2 - 1] + listaTempX[(GazeBufferX.Count / 2) + 0] + listaTempX[(GazeBufferX.Count / 2) + 1]) / 3;
                    gazeFiltered.Y = (listaTempY[GazeBufferX.Count / 2 - 1] + listaTempY[(GazeBufferX.Count / 2) + 0] + listaTempY[(GazeBufferX.Count / 2) + 1]) / 3;
                }
                return gazeFiltered;
            }
            else//si el buffer tiene menos de 3 elementos, se devuelve el argumento
            {
                return GazePoint;
            }
        }


        PointD getMovingAverageGaze(PointD GazePoint)
        {
            addPointD2Buffer(GazePoint, GazeBufferX, GazeBufferY, gazeBufferSize);

            var listaTempX = GazeBufferX.ToList();
            var listaTempY = GazeBufferY.ToList();

            PointD AveragePoint = new PointD();
            AveragePoint.X = listaTempX.Average();
            AveragePoint.Y = listaTempY.Average();
            
            return AveragePoint;
        }








        /// <summary>
        /// Busca si GazePoint está a una distancia mayor del CursorJumpThresholdNormalized
        /// para esto se mide la distancia entre: el nuevo gaze y la salida anterior del filtro
        /// si la distancia esta por encima de un umbral es un sacadico
        /// </summary>
        /// <param name="GazePoint"></param>
        /// <returns></returns>
        private bool gazeFix1(PointD GazePoint)
        {
            if (PointD.distance(GazePoint, lastFilterReturn) < CursorJumpThresholdNormalized)
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
        private bool gazeFix2(PointD GazePoint)
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
            if (buffer.Count == bufferSize)
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
