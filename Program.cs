//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System;
using System.Windows.Forms;
using Tobii.Gaze.Core;

namespace LookAndPlayForm
{ 
    public static class Program
    {
        static EyeTrackingEngine _eyeTrackingEngine;
        public static sharedData datosCompartidos;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            datosCompartidos = new sharedData();

            aclemottelibs.Wimu wimuDevice = new aclemottelibs.Wimu("COM52");

            if (!wimuDevice.serialPortConfigured)
            {
                MessageBox.Show("Head-tracking error!");
                return;
            }

            try
            {
                using (_eyeTrackingEngine = new EyeTrackingEngine())
                {
                    Application.Run(new EyeXWinForm(_eyeTrackingEngine, wimuDevice));
                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Eye-tracking error!");
            }                        
        }
    }
}
