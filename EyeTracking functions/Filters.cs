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

        public Filters()
        {
            FilterBufferSize = settings.filterBufferSize;
            GazeBufferX = new Queue<double>(FilterBufferSize);
            GazeBufferY = new Queue<double>(FilterBufferSize);
        }

        /// <summary>
        /// Ubica el cursor en la posicion especificada por posx y posy, donde estos valores estan normalizados 
        /// entre 0 y 1, siendo 0,0 la esquina superior izquierda y 1,1 la esquina inferior derecha
        /// </summary>
        /// <param name="posx"></param>
        /// <param name="posy"></param>
        /// <returns></returns>
        public PointD filterGazeData(PointD gazeData)
        {
            PointD gazeDataFiltered;

            if (Double.IsNaN(gazeData.X) || Double.IsNaN(gazeData.Y))
                gazeDataFiltered = new PointD(Double.NaN, Double.NaN);
            else
            {
                if (settings.filtertypeSelected == filtertype.movingaverage)
                    gazeDataFiltered = new PointD(getMovingAverageGaze(gazeData));
                if (settings.filtertypeSelected == filtertype.median)
                    gazeDataFiltered = new PointD(getMedianGazeFiltered(gazeData));
            }

            return gazeDataFiltered;
        }

        public bool clearBuffers()
        {
            GazeBufferX.Clear();
            GazeBufferY.Clear();
            return true;
        }

        /// <summary>
        ///1. si el argumento no tiene NANs
        ///         3. si el buffer esta lleno se filtra (movimientos de fijacion)
        ///             4. se saca un elemento de la cola y luego se pone el argumento
        ///             5. se transforma en lista la cola y se ordena de mayor a menor
        ///             6. se promedia los puntos del medio de la lista y se retorna este valor
        ///         7. si el buffer no esta lleno se encola el argumento y se retorna el argumento
        ///9. si el argumento tiene NANs se retorna el argumento        
        /// </summary>
        /// <param name="GazePoints"></param>
        /// <returns></returns>
        PointD getMedianGazeFiltered(PointD GazePoints)
        {    
            //1.si el argumento no tiene NANs
            if (!double.IsNaN(GazePoints.X) && !double.IsNaN(GazePoints.Y))
            {
                //3. si el buffer esta lleno se filtra (movimientos de fijacion)
                if (GazeBufferX.Count == FilterBufferSize || GazeBufferY.Count == FilterBufferSize)
                {
                    //4. se saca un elemento de la cola y luego se pone el argumento
                    GazeBufferX.Dequeue();
                    GazeBufferY.Dequeue();
                    GazeBufferX.Enqueue(GazePoints.X);
                    GazeBufferY.Enqueue(GazePoints.Y);
                    //5.se transforma en lista la cola y se ordena de mayor a menor
                    var listaTempX = GazeBufferX.ToList();
                    var listaTempY = GazeBufferY.ToList();
                    listaTempX.Sort();
                    listaTempY.Sort();

                    PointD MedianPoint = new PointD();
                    //6.se promedia los puntos del medio de la lista y se retorna este valor
                    MedianPoint.X = (listaTempX[FilterBufferSize / 2 - 1] + listaTempX[(FilterBufferSize / 2) + 0] + listaTempX[(FilterBufferSize / 2) + 1]) / 3;
                    MedianPoint.Y = (listaTempY[FilterBufferSize / 2 - 1] + listaTempY[(FilterBufferSize / 2) + 0] + listaTempY[(FilterBufferSize / 2) + 1]) / 3;
                    return MedianPoint;
                }
                //7.si el buffer no esta lleno se encola el argumento y se retorna el argumento
                else
                {
                    GazeBufferX.Enqueue(GazePoints.X);
                    GazeBufferY.Enqueue(GazePoints.Y);
                    return GazePoints;
                }
            }//9.si el argumento tiene NANs se retorna el argumento
            else
                return GazePoints;
        }       

        /// <summary>
        ///1.si el argumento no tiene NANs
        ///         3. si el buffer esta lleno
        ///             saca un elemento de la cola
        ///         4.1 mete argumento en la cola
        ///         4.2 se transforma en lista
        ///         4.3 se hace el promedio de la lista
        ///         4.4 se retorna el promedio de la lista
        /// 6. si el argumento tiene NANs
        ///     retorna argumento
        /// </summary>
        /// <param name="GazePoints"></param>
        /// <returns></returns>
        PointD getMovingAverageGaze(PointD GazePoints)
        {
            //1 si el argumento tiene NANs
            if (!double.IsNaN(GazePoints.X) && !double.IsNaN(GazePoints.Y))
            {
                //1.3.si el buffer esta lleno
                if (GazeBufferX.Count == FilterBufferSize || GazeBufferY.Count == FilterBufferSize)
                {
                    GazeBufferX.Dequeue();
                    GazeBufferY.Dequeue();
                }

                //4.1
                GazeBufferX.Enqueue(GazePoints.X);
                GazeBufferY.Enqueue(GazePoints.Y);
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
                return GazePoints;
            }
        }

    }
}
