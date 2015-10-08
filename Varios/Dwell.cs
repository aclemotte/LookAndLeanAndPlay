using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LookAndPlayForm
{
    class Dwell
    {
        Timer timerDwellLatency;
        Timer timerDwell;
        Queue<Point> CursorBuffer = new Queue<Point>();//buffer para el dwell click
        int buffersize_sam;
        Point avgPoint = new Point();
        Point stdPoint = new Point();
        MouseController controladorMouse;
        int DwellArea = settings.DwellArea;//pix
        int DwellTime = settings.DwellTime;//mseg
        int DwellLatency = settings.DwellLatency;

        public event EventHandler dwellClickFired;
        
        public Dwell()
        {
            timerDwell = new Timer();
            timerDwell.Interval = 33;
            timerDwell.Tick += new EventHandler(timerDwell_Tick);

            timerDwellLatency = new Timer();
            timerDwellLatency.Interval = DwellLatency;
            timerDwellLatency.Tick += new EventHandler(timerDwellLatency_Tick);

            buffersize_sam = (DwellTime / timerDwell.Interval);//1000 / 33 = 30

            controladorMouse = new MouseController();
        }

        /// <summary>
        /// Temporizador que muestrea la posicion del cursor y busca la condicion del click:
        ///     que la deviacion estandar del buffer sea menor a la definida
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerDwell_Tick(object sender, EventArgs e)
        {
            if (!LookAndPlayForm.Program.datosCompartidos.eyeNotFound)            
            {
                if (cursor_dwell())// && !dwelling)
                {
                    timerDwell.Enabled = false;
                    timerDwellLatency.Enabled = true;

                    //click!
                    generateClickEvent();
                }
            }
        }

        public void startDwelling()
        {
            timerDwell.Enabled = true;
        }

        public void stopDwelling()
        {
            timerDwell.Enabled = false;
            timerDwellLatency.Enabled = false;
            CursorBuffer.Clear();
        }

        /// <summary>
        /// definir un buffer de posiciones del cursor cuyo tamaño sea:
        ///         tiempo de dwelling / tiempo del temporizador
        /// ejemplo:   500ms       / 200 ms                   = 2
        /// 
        /// 1. si el buffer aun no se lleno, agregar un punto y retornar false
        /// 2. si el buffer se lleno, quitar una posicion del buffer de posiciones del cursor
        /// 3. agregar la posicion actual del cursor al buffer de posiciones del cursor
        /// 4. calculo de la medir
        /// 5. calculo de la varianza
        /// 6 .verificar si las varianzas del cursor en el buffer son menores a la distancia definida
        /// </summary>
        /// <returns></returns>
        private bool cursor_dwell()
        {
            //1. 
            if (CursorBuffer.Count < buffersize_sam)
            {
                CursorBuffer.Enqueue(new Point(Cursor.Position.X, Cursor.Position.Y));
                return false;
            }

            //2.
            CursorBuffer.Dequeue();

            //3. 
            CursorBuffer.Enqueue(new Point(Cursor.Position.X, Cursor.Position.Y));

            //4. 
            avgPoint = calculate_avg(CursorBuffer);

            //5.
            stdPoint = calculate_std(CursorBuffer, avgPoint);

            //6.
            if (stdPoint.X < DwellArea && stdPoint.Y < DwellArea)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Calcula la desviacion estandar del buffer
        /// std = raiz cuadrada ( sumatoria ( (cada punto - promedio ) ^ 2 ) / N ) 
        /// </summary>
        /// <param name="_CursorBuffer"></param>
        /// <returns></returns>
        private Point calculate_std(Queue<Point> _CursorBuffer, Point _avgPoint)
        {
            Point _stdPoint = new Point();

            foreach (Point puntoxy in _CursorBuffer)
            {
                _stdPoint.X += (puntoxy.X - _avgPoint.X) * (puntoxy.X - _avgPoint.X);
                _stdPoint.Y += (puntoxy.Y - _avgPoint.Y) * (puntoxy.Y - _avgPoint.Y);
            }

            _stdPoint.X = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(_stdPoint.X / _CursorBuffer.Count)));
            _stdPoint.Y = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(_stdPoint.Y / _CursorBuffer.Count)));

            return _stdPoint;
        }

        /// <summary>
        /// Calcula la media del buffer 
        /// </summary>
        /// <param name="_CursorBuffer"></param>
        /// <returns></returns>
        private Point calculate_avg(Queue<Point> _CursorBuffer)
        {
            Point _avgPoint = new Point();

            foreach (Point puntoxy in _CursorBuffer)
            {
                _avgPoint.X += puntoxy.X;
                _avgPoint.Y += puntoxy.Y;
            }

            _avgPoint.X = _avgPoint.X / _CursorBuffer.Count;
            _avgPoint.Y = _avgPoint.Y / _CursorBuffer.Count;

            return _avgPoint;
        }

        /// <summary>
        /// Temporizador que habilita el click segun su tiempo de tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerDwellLatency_Tick(object sender, EventArgs e)
        {
            timerDwellLatency.Enabled = false;
            limpiarBufferCursor();
            //dwelling = false;
            timerDwell.Enabled = true;
        }

        private void generateClickEvent()
        {
            controladorMouse.click();
            if (dwellClickFired != null) dwellClickFired(this, EventArgs.Empty);
        }

        private void limpiarBufferCursor()
        {
            CursorBuffer.Clear();
        }
    }
}
