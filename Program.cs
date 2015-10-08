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
            
            try
            {
                using (_eyeTrackingEngine = new EyeTrackingEngine())
                {
                    Application.Run(new EyeXWinForm(_eyeTrackingEngine));
                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error!");
            }                        
        }
    }
}
