using System;
using System.Drawing;
using System.Windows.Forms;
using FixDet;
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
        //FixDetectorClass fixationDetector;
        //bool gazeIsFix;
        private Filters gazeFilter = new Filters();
        long firtsTimeStampMicro;
        bool firstTimeStamp;



        public EyeXWinForm(EyeTrackingEngine eyeTrackingEngine)
        {
            firstTimeStamp = true;

            InitializeComponent();

            this.eyeTrackingEngine = eyeTrackingEngine;
            eyeTrackingEngine.GazePoint += this.GazePoint;
            eyeTrackingEngine.OnGetCalibrationCompletedEvent += this.OnGetCalibrationCompleted;
            eyeTrackingEngine.Initialize();

            _llkhk = new LowLevelKeyboardHook("Low-level Keyboard Hook");
            _llkhk.OnKeyPress += new EventHandler<KeyPressEventArgs>(OnKeyboardHookPress);

            clickDwell = new Dwell();


            //fixationDetector = new FixDetectorClass();
            //fixationDetector.FixationStart += fixationDetector_FixationStart;
            //fixationDetector.FixationEnd += fixationDetector_FixationEnd;
            //fixationDetector.FixationUpdate += fixationDetector_FixationUpdate;


            //fixationDetector.Analyzer = EFDAnalyzer.fdaFixationSize;
            //fixationDetector.FixationRadius = 70;
            //fixationDetector.NoiseFilter = 0;
            //fixationDetector.Filter = EFDFilter.fdfAveraging;
            //fixationDetector.MinFixDuration = 20;
            //fixationDetector.FilterBufferSize = 5;
            //fixationDetector.UpdateInterval = 1000;

            //fixationDetector.init();            

            //aclemottelibs.Wimu wimuDevice = new aclemottelibs.Wimu("COM37");
            //if (!wimuDevice.serialPortConfigured)
            //{
            //    MessageBox.Show("!wimuDevice.serialPortConfigured");
            //}

        }









        //void fixationDetector_FixationEnd(int aTime, int aDuration, int aX, int aY)
        //{
        //    textBoxFixation.BackColor = Color.Red;
        //    gazeFilter.clearBuffers();
        //    gazeIsFix = false;
        //}

        //void fixationDetector_FixationStart(int aTime, int aDuration, int aX, int aY)
        //{
        //    textBoxFixation.BackColor = Color.Green;
        //    gazeIsFix = true;
        //}
        	
        
        
        
        
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            eyeTrackingEngine.Dispose();
        }

        private void EyeXWinForm_Load(object sender, EventArgs e)
        {
            _llkhk.Install(); // keyboard
        }

        private void EyeXWinForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _llkhk.Uninstall();
            //fixationDetector.finalize();
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

            PointD gazeWeighted = eyetrackingFunctions.WeighGaze(gazePointEventArgs.GazeDataReceived);
            
            //fixationDetector.addPoint(
            //        convertirTimeStampMicro2Milli(gazePointEventArgs.GazeDataReceived.Timestamp),
            //        (int)(gazeWeighted.X * (double)Screen.PrimaryScreen.Bounds.Width),
            //        (int)(gazeWeighted.Y * (double)Screen.PrimaryScreen.Bounds.Height)
            //        );


            PointD cursorFiltered;
            if (AppControlCursor)
            {


                //if (fijacion())
                //    cursorLocation = GetCursorLocationFromHEADTracking();
                //else
                //    cursorLocation = GetCursorLocationFromEYETracking();

                //cursorLocation.locateCursor(cursorLocation);
                
                
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

        int convertirTimeStampMicro2Milli(long timeStampMicro)
        {
            int timeStampMili;

            if(firstTimeStamp)
            {
                firstTimeStamp = false;
                firtsTimeStampMicro = timeStampMicro;
                timeStampMili = 0;
            }
            else
            {
                timeStampMili = ((int)(timeStampMicro - firtsTimeStampMicro)) / 1000;
            }
            
            return timeStampMili;
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
