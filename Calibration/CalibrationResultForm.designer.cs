using LookAndPlayForm;

namespace LookAndPlayForm
{
    partial class CalibrationResultForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalibrationResultForm));
            this._okButton = new System.Windows.Forms.Button();
            this.textBoxCalibrationErrorLeft = new System.Windows.Forms.TextBox();
            this.textBoxCalibrationErrorRight = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._rightPlot = new LookAndPlayForm.CalibrationResultPanel();
            this._leftPlot = new LookAndPlayForm.CalibrationResultPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._okButton.Location = new System.Drawing.Point(680, 310);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(82, 27);
            this._okButton.TabIndex = 2;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // textBoxCalibrationErrorLeft
            // 
            this.textBoxCalibrationErrorLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCalibrationErrorLeft.Location = new System.Drawing.Point(328, 341);
            this.textBoxCalibrationErrorLeft.Name = "textBoxCalibrationErrorLeft";
            this.textBoxCalibrationErrorLeft.Size = new System.Drawing.Size(50, 26);
            this.textBoxCalibrationErrorLeft.TabIndex = 3;
            // 
            // textBoxCalibrationErrorRight
            // 
            this.textBoxCalibrationErrorRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCalibrationErrorRight.Location = new System.Drawing.Point(397, 341);
            this.textBoxCalibrationErrorRight.Name = "textBoxCalibrationErrorRight";
            this.textBoxCalibrationErrorRight.Size = new System.Drawing.Size(50, 26);
            this.textBoxCalibrationErrorRight.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(397, 316);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Right";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(341, 316);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Left";
            // 
            // _rightPlot
            // 
            this._rightPlot.BackColor = System.Drawing.Color.WhiteSmoke;
            this._rightPlot.EyeOption = LookAndPlayForm.EyeOption.Right;
            this._rightPlot.Location = new System.Drawing.Point(397, 12);
            this._rightPlot.Name = "_rightPlot";
            this._rightPlot.Size = new System.Drawing.Size(366, 275);
            this._rightPlot.TabIndex = 1;
            this._rightPlot.Text = "calibrationResultPanel2";
            // 
            // _leftPlot
            // 
            this._leftPlot.BackColor = System.Drawing.Color.WhiteSmoke;
            this._leftPlot.EyeOption = LookAndPlayForm.EyeOption.Left;
            this._leftPlot.Location = new System.Drawing.Point(12, 12);
            this._leftPlot.Name = "_leftPlot";
            this._leftPlot.Size = new System.Drawing.Size(366, 275);
            this._leftPlot.TabIndex = 0;
            this._leftPlot.Text = "calibrationResultPanel1";
            // 
            // groupBox1
            // 
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(304, 293);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(176, 83);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Calibration error (px)";
            // 
            // CalibrationResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 388);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxCalibrationErrorRight);
            this.Controls.Add(this.textBoxCalibrationErrorLeft);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._rightPlot);
            this.Controls.Add(this._leftPlot);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalibrationResultForm";
            this.Text = "Calibration Result";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CalibrationResultPanel _leftPlot;
        private CalibrationResultPanel _rightPlot;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.TextBox textBoxCalibrationErrorLeft;
        private System.Windows.Forms.TextBox textBoxCalibrationErrorRight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}