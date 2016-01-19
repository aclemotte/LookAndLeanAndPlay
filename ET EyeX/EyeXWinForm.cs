using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using LookAndPlayForm.Fusionador;
using LookAndPlayForm.HT;
using Tobii.Gaze.Core;
using WobbrockLib.Devices;


namespace LookAndPlayForm
{


    public partial class EyeXWinForm : Form
    {

        public readonly EyeTrackingEngine eyeTrackingEngine;

        bool AppControlCursor = false;
        MouseController CursorControl = new MouseController();
        LowLevelKeyboardHook _llkhk;
        Dwell clickDwell;
        Filters gazeFilter = new Filters();

        int pointsInChart = 100;

        Head2deltaCursor head2deltaCursor;
        int PseudoTimeStampMiliSecImu;
        WimuData headData;
        HT.Wimu wimuDevice;

        public EyeXWinForm(EyeTrackingEngine eyeTrackingEngine, HT.Wimu wimuDevice)
        {
            PseudoTimeStampMiliSecImu = 0;

            InitializeComponent();

            this.eyeTrackingEngine = eyeTrackingEngine;
            eyeTrackingEngine.GazePoint += this.GazePoint;
            eyeTrackingEngine.OnGetCalibrationCompletedEvent += this.OnGetCalibrationCompleted;
            eyeTrackingEngine.Initialize();

            _llkhk = new LowLevelKeyboardHook("Low-level Keyboard Hook");
            _llkhk.OnKeyPress += new EventHandler<KeyPressEventArgs>(OnKeyboardHookPress);

            clickDwell = new Dwell();

            this.wimuDevice = wimuDevice;
            this.head2deltaCursor = new Head2deltaCursor(wimuDevice);            

        }









        	
        
        
        
        
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            eyeTrackingEngine.Dispose();
            wimuDevice.Dispose();
        }









        private void EyeXWinForm_Load(object sender, EventArgs e)
        {
            _llkhk.Install(); // keyboard
        }

        private void EyeXWinForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _llkhk.Uninstall();
        }










        private delegate void Action();

        private void GazePoint(object sender, GazePointEventArgs gazePointEventArgs)
        {
            BeginInvoke(new Action(() =>
            {
                var handle = Handle;
                if (handle == null)
                {
                    // window not created yet. never mind.
                    return;
                }

                _trackStatus.OnGazeData(gazePointEventArgs.GazeDataReceived);
                progressBar4Distance.Value = eyetrackingFunctions.distanceBetweenDev2User(gazePointEventArgs.GazeDataReceived);
                Invalidate();
            }));




            PointD gazeWeighted = eyetrackingFunctions.WeighGaze(gazePointEventArgs.GazeDataReceived);//valores normalizados






            headData = wimuDevice.WimuData;

            //point2Chart(new PointD(headData.timeStampMiliSec, headData.yaw));
            //point2Chart(new PointD(PseudoTimeStampMiliSecImu++, headData.yaw));
            headData.timeStampMiliSec = (double)PseudoTimeStampMiliSecImu++;
            headData2Chart(headData);





            if (AppControlCursor)
            {
                PointD deltaCursor = head2deltaCursor.GetDeltaLocationFromHEADTracking();
                //PointD deltaCursor = new PointD(0, 0);
                PointD gazeFilteredNormalized = gazeFilter.filterGazeData(gazeWeighted);//valores normalizados
                PointD gazeFilteredPixels = eyetrackingFunctions.normalized2Pixels(gazeFilteredNormalized);
                Point cursorLocation = (Point)fusionador.getCursorLocation(true, deltaCursor, gazeFilteredPixels);                
                CursorControl.locateCursor(cursorLocation);


                //cursorFiltered = gazeFilter.filterGazeData(gazeWeighted);
                //CursorControl.locateCursor(cursorFiltered);



                //if (gazeIsFix)
                //{
                //    cursorFiltered = gazeFilter.filterGazeData(gazeWeighted);
                //    CursorControl.locateCursor(cursorFiltered);
                //    //CursorControl.locateCursor(gazeWeighted);
                //}
                //else
                //{
                //    CursorControl.locateCursor(gazeWeighted);
                //}
            }
        }





        private void headData2Chart(WimuData headData)
        {
            point2Chart(new PointD(headData.timeStampMiliSec, headData.yaw), chartYaw);
            point2Chart(new PointD(headData.timeStampMiliSec, headData.pitch), chartPitch);
            point2Chart(new PointD(headData.timeStampMiliSec, headData.roll), chartRoll);
        }

        delegate void AddDataToChartDelegate(PointD newPoint, Chart chartName);

        private void point2Chart(PointD newPoint, Chart chartName)
        {


            if (chartName.InvokeRequired)
                chartName.Invoke(new AddDataToChartDelegate(this.point2Chart), new object[] { newPoint, chartName });
            else
            {
                chartName.Series["Series1"].Points.AddXY(newPoint.X, newPoint.Y);

                if (chartName.Series["Series1"].Points.Count > pointsInChart)
                {
                    chartName.Series["Series1"].Points.RemoveAt(0);
                }

                chartName.ChartAreas["ChartArea1"].AxisX.Minimum = chartName.Series["Series1"].Points[0].XValue;
                chartName.ChartAreas["ChartArea1"].AxisX.Maximum = chartName.Series["Series1"].Points[chartName.Series["Series1"].Points.Count-1].XValue;

            }
        }






        private void OnGetCalibrationCompleted(object sender, CalibrationReadyEventArgs e)
        {
            textBoxCalibrationErrorLeft.BeginInvoke((Action)(() =>
            {
                if (LookAndPlayForm.Program.datosCompartidos.meanCalibrationErrorLeftPx == -2147483648)
                    textBoxCalibrationErrorLeft.Text = "No calibrado";
                else
                    textBoxCalibrationErrorLeft.Text = LookAndPlayForm.Program.datosCompartidos.meanCalibrationErrorLeftPx.ToString();
            }));

            textBoxCalibrationErrorRight.BeginInvoke((Action)(() =>
            {
                if (LookAndPlayForm.Program.datosCompartidos.meanCalibrationErrorRightPx == -2147483648)
                    textBoxCalibrationErrorRight.Text = "No calibrado";
                else
                    textBoxCalibrationErrorRight.Text = LookAndPlayForm.Program.datosCompartidos.meanCalibrationErrorRightPx.ToString();
            }));

        }







        //Teclado cambia control
        private void OnKeyboardHookPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'q':
                case 'Q':
                    toogleETcontrolCursor();
                    break;

            }
        }

        private void toogleETcontrolCursor()
        {
            AppControlCursor = !AppControlCursor;

            //if (AppControlCursor)
            //    clickDwell.startDwelling();

            //if (!AppControlCursor)
            //    clickDwell.stopDwelling();
        }









        //Botones
        private void buttonViewCalibration_Click(object sender, EventArgs e)
        {
            var resultForm = new CalibrationResultForm();
            resultForm.SetPlotData(LookAndPlayForm.Program.datosCompartidos.calibrationDataEyeX);
            resultForm.ShowDialog();
        }

        private void buttonCalibrate_Click(object sender, EventArgs e)
        {
            CalibrationWinForm calibrationForm = new CalibrationWinForm(this, eyeTrackingEngine);
            calibrationForm.Show();
        }

        
        
        
    }
}
