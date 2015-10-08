namespace LookAndPlayForm
{
    partial class CalibrationWinForm
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
            this.pictureBoxCalibrationImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCalibrationImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxCalibrationImage
            // 
            this.pictureBoxCalibrationImage.Location = new System.Drawing.Point(69, 54);
            this.pictureBoxCalibrationImage.Name = "pictureBoxCalibrationImage";
            this.pictureBoxCalibrationImage.Size = new System.Drawing.Size(100, 100);
            this.pictureBoxCalibrationImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCalibrationImage.TabIndex = 0;
            this.pictureBoxCalibrationImage.TabStop = false;
            // 
            // CalibrationWinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 564);
            this.Controls.Add(this.pictureBoxCalibrationImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CalibrationWinForm";
            this.Text = "CalibrationWinForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CalibrationWinForm_FormClosed);
            this.Load += new System.EventHandler(this.CalibrationWinForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCalibrationImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxCalibrationImage;
    }
}