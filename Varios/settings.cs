using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LookAndPlayForm
{
    class settings
    {
        //eyesDetector
        public const int maxNumberOfDataReceived = 15;
        
        public const int DwellArea = 50;//0pix
        public const int DwellTime = 1000;//mseg
        public const int DwellLatency = 1000;

        public const int maxNumberOfSerialReads = 10;
        public const string headTrackerSerialPort = "COM5";
        //WIMU blanco 20:14:02:18:11:01

        //puntero
        public const pointercontroltype pointercontroltypeSelected = pointercontroltype.eyetracker;
        //click
        public const clictype clictypeSelected = clictype.dwell;

        public const filtertype filtertypeSelected = filtertype.median;
        public const int filterBufferSize = 15;//numero impar mayor a 3
        public const double thresholdFilterNormalized = 2000;//numero de pixeles

        public const eyetrackertype eyetrackerSelected = eyetrackertype.tobii;
    }
}
