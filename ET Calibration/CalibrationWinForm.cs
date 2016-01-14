using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LookAndPlayForm
{
    public partial class CalibrationWinForm : Form
    {

        struct calibrationPoint
        {
            public Tobii.Gaze.Core.Point2D positionEyeX_Normalizada;
            public LookAndPlayForm.Resumen.Point2D positionPx;
            public Bitmap imagen;
        };

        private enum tamanhoImagen
        {
            big,
            little
        };

        Queue<calibrationPoint> calibrationPoints = new Queue<calibrationPoint>();



        private const int samplingDataTime = 750;
        private const int idleTime = 1000; //2000;
        //EyeX
        private const int preShowTime = 500;
        private const int postShowTime = 1000;
        
        private int calibrationPointOffset = 100;
        private int numeroPuntosCalibracion;
        private bool eyeTrackerReady2Calibrate = false;
        private Size bigImage = new Size(80, 80);
        private Size littleImage = new Size(20, 20);
        private Timer timerPreShow;
        private Timer timerPostShow;
        private calibrationPoint currentCalibrationPoint;
        private EyeXWinForm eyeXWinForm;
        private static readonly Random Random = new Random();
        private EyeTrackingEngine eyeTrackingEngine;

        public CalibrationWinForm(EyeXWinForm eyeXWinForm, EyeTrackingEngine eyeTrackingEngine)
        {
            InitializeComponent();

            Cursor.Hide();

            this.eyeXWinForm = eyeXWinForm;
            this.eyeTrackingEngine = eyeTrackingEngine;

            eyeTrackingEngine.onStartCalibrationCompletedEvent += this.onStartCalibrationCompleted;
            eyeTrackingEngine.onAddCalibrationPointCompletedEvent += this.onAddCalibrationPointCompleted;
            eyeTrackingEngine.onComputeandSetCalibrationCompletedEvent += this.onComputeandSetCalibrationCompleted;
            eyeTrackingEngine.OnGetCalibrationCompletedEvent += this.OnGetCalibrationCompleted;

            configTimers();
            generateCalibrationPoints();
            requestStartCalibrationEyeX();

        }

        private void CalibrationWinForm_Load(object sender, EventArgs e)
        {
            pictureBoxCalibrationImage.Size = bigImage;
            currentCalibrationPoint = setCalibrationPoint();

            if (eyeTrackerReady2Calibrate && (settings.eyetrackerSelected == Definitions.eyetrackertype.tobii))
            {
                startTimersEyeX();
            }
        }




        private void configTimers()
        {            
            timerPreShow = new Timer();
            timerPreShow.Interval = preShowTime;
            timerPreShow.Tick += new EventHandler(timerPreShow_Tick_EyeX);

            timerPostShow = new Timer();
            timerPostShow.Interval = postShowTime;
            timerPostShow.Tick += new EventHandler(timerPostShow_Tick_EyeX);
        }

        /// <summary>
        /// Define la posicion del centro de los puntos de calibracion
        /// </summary>
        private void generateCalibrationPoints()
        {
            int Width = Screen.AllScreens[0].Bounds.Width;
            int Height = Screen.AllScreens[0].Bounds.Height;
            double offsetX = (double)calibrationPointOffset / (double)Width;
            double offsetY = (double)calibrationPointOffset / (double)Height;
            calibrationPoint puntoCalibracion = new calibrationPoint();
            List<calibrationPoint> puntoCalibracionLista = new List<calibrationPoint>();

            /*
             * (x,y)
             * 
             *  1(offset,offset)        2(Width/2,offset)           3(Width-offset,offset)
             *  4(offset,Height/2)      5(Width/2,Height/2)         6(Width-offset,Height/2)
             *  7(offset,Height-offset) 8(Width/2,Height-offset)    9(Width-offset,Height-offset)
             */

            puntoCalibracion.positionPx = new LookAndPlayForm.Resumen.Point2D();//1
            puntoCalibracion.positionPx.X = calibrationPointOffset;
            puntoCalibracion.positionPx.Y = calibrationPointOffset;
            puntoCalibracion.positionEyeX_Normalizada = new Tobii.Gaze.Core.Point2D(offsetX, offsetY);
            puntoCalibracion.imagen = Properties.Resources.cocodrilo;
            puntoCalibracionLista.Add(puntoCalibracion);

            //puntoCalibracion.positionEyeTribe = new Point2D(Width / 2, calibrationPointOffset);//2
            //puntoCalibracion.positionEyeX = new Tobii.Gaze.Core.Point2D(0.5, offsetY);
            //puntoCalibracion.imagen = Properties.Resources.tigre;
            //puntoCalibracionLista.Add(puntoCalibracion);

            puntoCalibracion.positionPx = new LookAndPlayForm.Resumen.Point2D();//3
            puntoCalibracion.positionPx.X = Width - calibrationPointOffset;
            puntoCalibracion.positionPx.Y = calibrationPointOffset;
            puntoCalibracion.positionEyeX_Normalizada = new Tobii.Gaze.Core.Point2D(1 - offsetX, offsetY);
            puntoCalibracion.imagen = Properties.Resources.elefante;
            puntoCalibracionLista.Add(puntoCalibracion);

            //puntoCalibracion.positionEyeTribe = new Point2D(calibrationPointOffset, Height / 2);//4
            //puntoCalibracion.positionEyeX = new Tobii.Gaze.Core.Point2D(offsetX, 0.5);
            //puntoCalibracion.imagen = Properties.Resources.mono;
            //puntoCalibracionLista.Add(puntoCalibracion);

            puntoCalibracion.positionPx = new LookAndPlayForm.Resumen.Point2D();//5
            puntoCalibracion.positionPx.X = Width / 2;
            puntoCalibracion.positionPx.Y = Height / 2;
            puntoCalibracion.positionEyeX_Normalizada = new Tobii.Gaze.Core.Point2D(0.5, 0.5);
            puntoCalibracion.imagen = Properties.Resources.sapo;
            puntoCalibracionLista.Add(puntoCalibracion);

            //puntoCalibracion.positionEyeTribe = new Point2D(Width - calibrationPointOffset, Height / 2);//6
            //puntoCalibracion.positionEyeX = new Tobii.Gaze.Core.Point2D(1 - offsetX, 0.5);
            //puntoCalibracion.imagen = Properties.Resources.pavo;
            //puntoCalibracionLista.Add(puntoCalibracion);

            puntoCalibracion.positionPx = new LookAndPlayForm.Resumen.Point2D();//7
            puntoCalibracion.positionPx.X = calibrationPointOffset;
            puntoCalibracion.positionPx.Y = Height - calibrationPointOffset;
            puntoCalibracion.positionEyeX_Normalizada = new Tobii.Gaze.Core.Point2D(offsetX, 1 - offsetY);
            puntoCalibracion.imagen = Properties.Resources.pez;
            puntoCalibracionLista.Add(puntoCalibracion);

            //puntoCalibracion.positionEyeTribe = new Point2D(Width / 2, Height - calibrationPointOffset);//8
            //puntoCalibracion.positionEyeX = new Tobii.Gaze.Core.Point2D(0.5, 1 - offsetY);
            //puntoCalibracion.imagen = Properties.Resources.raton;
            //puntoCalibracionLista.Add(puntoCalibracion);

            puntoCalibracion.positionPx = new LookAndPlayForm.Resumen.Point2D();//9
            puntoCalibracion.positionPx.X = Width - calibrationPointOffset;
            puntoCalibracion.positionPx.Y = Height - calibrationPointOffset;
            puntoCalibracion.positionEyeX_Normalizada = new Tobii.Gaze.Core.Point2D(1 - offsetX, 1 - offsetY);
            puntoCalibracion.imagen = Properties.Resources.tortuga;
            puntoCalibracionLista.Add(puntoCalibracion);



            numeroPuntosCalibracion = puntoCalibracionLista.Count;
            //int[] order = new int[numeroPuntosCalibracion];            
            int[] order = new int[numeroPuntosCalibracion];

            for (var c = 0; c < numeroPuntosCalibracion; c++)
                order[c] = c;

            Shuffle(order);

            foreach (int number in order)
            {
                puntoCalibracion = puntoCalibracionLista[number];
                calibrationPoints.Enqueue(puntoCalibracion);
            }

        }
        
        private bool requestStartCalibrationEyeX()
        {
            eyeTrackingEngine.requestStartCalibration();
            return true;
        }







        private calibrationPoint setCalibrationPoint()
        {
            calibrationPoint calibrationPointSelected;

            if (calibrationPoints.Count != 0)
            {
                calibrationPointSelected = calibrationPoints.Dequeue();
                relocateCalibrationImage(calibrationPointSelected);
                return calibrationPointSelected;
            }
            else
            {
                calibrationPointSelected = new calibrationPoint();
                calibrationPointSelected.imagen = null;
                return calibrationPointSelected;
            }
        }

        private void startTimersEyeX()
        {
            this.BeginInvoke((Action)(() =>
            {
                timerPreShow.Start();
            }));
        }




        //Timers EyeX
        private void timerPreShow_Tick_EyeX(object sender, EventArgs e)
        {
            timerPreShow.Stop();
            resizeImage(tamanhoImagen.little);
            eyeTrackingEngine.AddCalibrationPoint(currentCalibrationPoint.positionEyeX_Normalizada);            
        }

        private void timerPostShow_Tick_EyeX(object sender, EventArgs e)
        {
            timerPostShow.Stop();            
            currentCalibrationPoint = setCalibrationPoint();

            if (currentCalibrationPoint.imagen != null)
            {
                timerPreShow.Start();            
            }
            else
            {
                timerPreShow.Tick -= new EventHandler(timerPreShow_Tick_EyeX);
                timerPostShow.Tick -= new EventHandler(timerPostShow_Tick_EyeX);

                Console.WriteLine("CalibrationWinForm.timerLatency_Tick_EyeX: final");                
                eyeTrackingEngine.ComputeAndSetCalibration();
            }            
        }






        private void closeProtocol()
        {

            this.BeginInvoke((Action)(() =>
            {
                this.Close();
            }));
        }

        private void resizeImage(tamanhoImagen imageSize)
        {
            if (imageSize == tamanhoImagen.big)
            {
                pictureBoxCalibrationImage.BeginInvoke((Action)(() =>
                {
                    pictureBoxCalibrationImage.Size = bigImage;
                    pictureBoxCalibrationImage.Location = new Point(
                        Convert.ToInt32(pictureBoxCalibrationImage.Location.X - ((bigImage.Width / 2) - (littleImage.Width / 2))),
                        Convert.ToInt32(pictureBoxCalibrationImage.Location.Y - ((bigImage.Height / 2) - (littleImage.Height / 2)))
                        );
                }));
            }

            if (imageSize == tamanhoImagen.little)
            {
                pictureBoxCalibrationImage.BeginInvoke((Action)(() =>
                {
                    pictureBoxCalibrationImage.Size = littleImage;
                    pictureBoxCalibrationImage.Location = new Point(
                        Convert.ToInt32(pictureBoxCalibrationImage.Location.X + ((bigImage.Width / 2) - (littleImage.Width / 2))),
                        Convert.ToInt32(pictureBoxCalibrationImage.Location.Y + ((bigImage.Height / 2) - (littleImage.Height / 2)))
                        );
                }));
            }
        }

        private void relocateCalibrationImage(calibrationPoint calibrationPoint)
        {
            pictureBoxCalibrationImage.BeginInvoke((Action)(() =>
            {
                pictureBoxCalibrationImage.Location = new Point(
                    Convert.ToInt32(calibrationPoint.positionPx.X) - (pictureBoxCalibrationImage.Width / 2),
                    Convert.ToInt32(calibrationPoint.positionPx.Y) - (pictureBoxCalibrationImage.Height / 2)
                    );
                pictureBoxCalibrationImage.Image = calibrationPoint.imagen;
            }));
        }

        private static void Shuffle<T>(IList<T> array)
        {
            if (array == null)
                return;

            var random = Random;

            for (var i = array.Count; i > 1; i--)
            {
                var j = random.Next(i);
                var tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }




        //Eventos del EyeX
        private void onStartCalibrationCompleted(object sender, EventArgs e)
        {
            eyeTrackerReady2Calibrate = true;

            if(this.Visible)
            {
                Console.WriteLine("CalibrationWinForm.onStartCalibrationCompleted. this.Visible = true");
                startTimersEyeX();
            }
            else
            {
                Console.WriteLine("CalibrationWinForm.onStartCalibrationCompleted. this.Visible = false");
            }
        }

        private void onAddCalibrationPointCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("CalibrationWinForm.onAddCalibrationPointCompleted");
            resizeImage(tamanhoImagen.big);

            this.BeginInvoke((Action)(() =>
            {
                timerPostShow.Start();
            }));
            
        }

        private void onComputeandSetCalibrationCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("CalibrationWinForm.onComputeandSetCalibrationCompleted");
            closeProtocol();
        }

        private void OnGetCalibrationCompleted(object sender, CalibrationReadyEventArgs e)
        {
             
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
                            @"\calibrationDataEyeX_" +
                            Varios.functions.getCurrentTime() +
                            ".json";
                            
            System.IO.File.WriteAllText(
                path,
                Newtonsoft.Json.JsonConvert.SerializeObject(e.CalibrationPointDataL)
                );
        }

        private void CalibrationWinForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            eyeTrackingEngine.onStartCalibrationCompletedEvent -= this.onStartCalibrationCompleted;
            eyeTrackingEngine.onAddCalibrationPointCompletedEvent -= this.onAddCalibrationPointCompleted;
            eyeTrackingEngine.onComputeandSetCalibrationCompletedEvent -= this.onComputeandSetCalibrationCompleted;
            eyeTrackingEngine.OnGetCalibrationCompletedEvent -= this.OnGetCalibrationCompleted;

            Cursor.Show();
        }
        
    }
}
