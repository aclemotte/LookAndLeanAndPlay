using System;
using System.Drawing;
using System.Windows.Forms;
using Tobii.Gaze.Core;
using WobbrockLib.Devices;


namespace LookAndPlayForm
{


    public partial class EyeXWinForm : Form
    {

        public readonly EyeTrackingEngine eyeTrackingEngine;

        private bool ETcontrolCursor = false;
        private MouseController CursorControl = new MouseController();
        private LowLevelKeyboardHook _llkhk;
        private Dwell clickDwell;





        public EyeXWinForm(EyeTrackingEngine eyeTrackingEngine)
        {
            InitializeComponent();

            this.eyeTrackingEngine = eyeTrackingEngine;
            eyeTrackingEngine.GazePoint += this.GazePoint;
            eyeTrackingEngine.OnGetCalibrationCompletedEvent += this.OnGetCalibrationCompleted;

            eyeTrackingEngine.Initialize();

            _llkhk = new LowLevelKeyboardHook("Low-level Keyboard Hook");
            _llkhk.OnKeyPress += new EventHandler<KeyPressEventArgs>(OnKeyboardHookPress);

            clickDwell = new Dwell();

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

            if (ETcontrolCursor)
            {
                PointD cursorFiltered = new PointD();
                PointD gazeWeighted;
                gazeWeighted = eyetrackingFunctions.WeighGaze(gazePointEventArgs.GazeDataReceived);

                if (ETcontrolCursor)
                    cursorFiltered = CursorControl.filterData(gazeWeighted, true);
                else
                    cursorFiltered = CursorControl.filterData(gazeWeighted, false);
            }
        }
        	
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            eyeTrackingEngine.Dispose();
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

        private void EyeXWinForm_Load(object sender, EventArgs e)
        {
            _llkhk.Install(); // keyboard
        }

        private void EyeXWinForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _llkhk.Uninstall();
        }

        private void toogleETcontrolCursor()
        {
            ETcontrolCursor = !ETcontrolCursor;
            
            if(ETcontrolCursor)
                clickDwell.startDwelling();

            if(!ETcontrolCursor)
                clickDwell.stopDwelling();
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
    }
}
