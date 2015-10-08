using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tobii.Gaze.Core;

namespace LookAndPlayForm
{
    public partial class CalibrationResultForm : Form
    {
        sharedData datosCompartidos;

        public CalibrationResultForm()
        {
            this.datosCompartidos = LookAndPlayForm.Program.datosCompartidos;
            InitializeComponent();
        }

        public void SetPlotData(Tobii.Gaze.Core.Calibration calibration)
        {
            _leftPlot.Initialize(calibration.GetCalibrationPointDataItems());
            _rightPlot.Initialize(calibration.GetCalibrationPointDataItems());
            
            if (datosCompartidos.meanCalibrationErrorLeftPx == -2147483648)
                textBoxCalibrationErrorLeft.Text = "No calibrado";
            else
                textBoxCalibrationErrorLeft.Text = datosCompartidos.meanCalibrationErrorLeftPx.ToString();

            if (LookAndPlayForm.Program.datosCompartidos.meanCalibrationErrorRightPx == -2147483648)
                textBoxCalibrationErrorRight.Text = "No calibrado";
            else
                textBoxCalibrationErrorRight.Text = datosCompartidos.meanCalibrationErrorRightPx.ToString();
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}