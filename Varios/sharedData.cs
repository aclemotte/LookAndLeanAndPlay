using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tobii.Gaze.Core;

namespace LookAndPlayForm
{
    //el que instancio esta clase es EyeTrackingEngine
    public class sharedData
    {
        public int meanCalibrationErrorLeftPx { get; set; }
        public int meanCalibrationErrorRightPx { get; set; }
        public string EyeTrackerInfo { get; set; }
        public Tobii.Gaze.Core.Calibration calibrationDataEyeX { get; set; }
        public bool eyeNotFound { get; set; }




        public sharedData()
        {
            eyeNotFound = true;
        }
    }
}
