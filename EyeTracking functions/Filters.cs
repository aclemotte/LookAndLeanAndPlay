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

        //double lastRawX = 0;
        //double lastRawY = 0;
        //PointF PointFilteredPID = new PointF(0,0);
        const double FDeadZone = 0.020;//17; // pixels
        const double g_SlowZone = 0.040;//1; // pixels
        const double FSpeed = 0.8f; // how fast to move when far away
        const double FSlowSpeed = 0.00001f; // how fast to move when close

        //public struct PointF
        //{
        //    public double X;
        //    public double Y;
        //};

        public Filters()
        {
            FilterBufferSize = settings.filterBufferSize;
            GazeBufferX = new Queue<double>(FilterBufferSize);
            GazeBufferY = new Queue<double>(FilterBufferSize);
        }

        /// <summary>
        ///1. si el argumento no tiene NANs
        ///     2. si el error es pequeño se filtra (movimientos de fijacion)
        ///         3. si el buffer esta lleno se filtra (movimientos de fijacion)
        ///             4. se saca un elemento de la cola y luego se pone el argumento
        ///             5. se transforma en lista la cola y se ordena de mayor a menor
        ///             6. se promedia los puntos del medio de la lista y se retorna este valor
        ///         7. si el buffer no esta lleno se encola el argumento y se retorna el argumento
        ///     8. si el error es grande se limpian los bufferes y se retorna el argumento
        ///9. si el argumento tiene NANs se retorna el argumento        
        /// </summary>
        /// <param name="GazePoints"></param>
        /// <returns></returns>
        public PointD getMedianGazeFiltered(PointD GazePoints)
        {    
            //1.si el argumento no tiene NANs
            if (!double.IsNaN(GazePoints.X) && !double.IsNaN(GazePoints.Y))
            {
                //2. si el error es pequeño se filtra (movimientos de fijacion)
                if (getEtCursorDifference(GazePoints) < settings.thresholdFilterNormalized)
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
                }//8.si el error es grande se limpian los bufferes y se retorna el argumento
                else
                {
                    GazeBufferX.Clear();
                    GazeBufferY.Clear();
                    return GazePoints;
                }
            }//9.si el argumento tiene NANs se retorna el argumento
            else
                return GazePoints;
        }       

        /// <summary>
        ///1.si el argumento no tiene NANs
        ///     2.si el error es pequenho se filtra (movimientos de fijacion)
        ///         3. si el buffer esta lleno
        ///             saca un elemento de la cola
        ///         4.1 mete argumento en la cola
        ///         4.2 se transforma en lista
        ///         4.3 se hace el promedio de la lista
        ///         4.4 se retorna el promedio de la lista
        ///     5.si el error es grande no se filtra (movimientos sacadicos
        ///         se limpian los buferes
        ///         se retorna el argumento
        /// 6. si el argumento tiene NANs
        ///     retorna argumento
        /// </summary>
        /// <param name="GazePoints"></param>
        /// <returns></returns>
        public PointD getMovingAverageGaze(PointD GazePoints)
        {
            //1 si el argumento tiene NANs
            if (!double.IsNaN(GazePoints.X) && !double.IsNaN(GazePoints.Y))
            {
                //1.2.si el error es pequeño se filtra (movimientos de fijacion)
                if (getEtCursorDifference(GazePoints) < settings.thresholdFilterNormalized)
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

                }//sino 5
                else
                {
                    GazeBufferX.Clear();
                    GazeBufferY.Clear();
                    return GazePoints;
                }
            }//sino 6
            else
            {
                return GazePoints;
            }
        }

        /// <summary>
        /// Calcula la distancia entre la posicion de la mirada y la posicion del cursor en px
        /// </summary>
        /// <param name="GazePoint"></param>
        /// <returns></returns>
        double getEtCursorDifference(PointD GazePoint)
        {
            double errorX = Math.Pow((Cursor.Position.X - (GazePoint.X* Screen.PrimaryScreen.Bounds.Width)),2);
            double errorY = Math.Pow((Cursor.Position.Y - (GazePoint.Y* Screen.PrimaryScreen.Bounds.Height)),2);
            double error = Math.Sqrt(errorX + errorY);
            return error;
        }
    }
}
