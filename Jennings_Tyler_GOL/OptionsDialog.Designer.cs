
namespace Jennings_Tyler_GOL
{
    partial class OptionsDialog
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.universeWidthUpDown = new System.Windows.Forms.NumericUpDown();
            this.universeHeightUpDown = new System.Windows.Forms.NumericUpDown();
            this.widthLabel = new System.Windows.Forms.Label();
            this.heightLabel = new System.Windows.Forms.Label();
            this.intervalLabel = new System.Windows.Forms.Label();
            this.intervalUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.universeWidthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.universeHeightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intervalUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(280, 226);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(361, 226);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // universeWidthUpDown
            // 
            this.universeWidthUpDown.Location = new System.Drawing.Point(232, 121);
            this.universeWidthUpDown.Name = "universeWidthUpDown";
            this.universeWidthUpDown.Size = new System.Drawing.Size(120, 20);
            this.universeWidthUpDown.TabIndex = 2;
            // 
            // universeHeightUpDown
            // 
            this.universeHeightUpDown.Location = new System.Drawing.Point(232, 145);
            this.universeHeightUpDown.Name = "universeHeightUpDown";
            this.universeHeightUpDown.Size = new System.Drawing.Size(120, 20);
            this.universeHeightUpDown.TabIndex = 3;
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Location = new System.Drawing.Point(92, 121);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(128, 13);
            this.widthLabel.TabIndex = 4;
            this.widthLabel.Text = "Width of Universe in Cells";
            // 
            // heightLabel
            // 
            this.heightLabel.AutoSize = true;
            this.heightLabel.Location = new System.Drawing.Point(92, 147);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(131, 13);
            this.heightLabel.TabIndex = 5;
            this.heightLabel.Text = "Height of Universe in Cells";
            // 
            // intervalLabel
            // 
            this.intervalLabel.AutoSize = true;
            this.intervalLabel.Location = new System.Drawing.Point(92, 96);
            this.intervalLabel.Name = "intervalLabel";
            this.intervalLabel.Size = new System.Drawing.Size(139, 13);
            this.intervalLabel.TabIndex = 7;
            this.intervalLabel.Text = "Time Interval in Milliseconds";
            // 
            // intervalUpDown
            // 
            this.intervalUpDown.Location = new System.Drawing.Point(232, 96);
            this.intervalUpDown.Name = "intervalUpDown";
            this.intervalUpDown.Size = new System.Drawing.Size(120, 20);
            this.intervalUpDown.TabIndex = 6;
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(444, 261);
            this.Controls.Add(this.intervalLabel);
            this.Controls.Add(this.intervalUpDown);
            this.Controls.Add(this.heightLabel);
            this.Controls.Add(this.widthLabel);
            this.Controls.Add(this.universeHeightUpDown);
            this.Controls.Add(this.universeWidthUpDown);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OptionsDialog";
            ((System.ComponentModel.ISupportInitialize)(this.universeWidthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.universeHeightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intervalUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.NumericUpDown universeWidthUpDown;
        private System.Windows.Forms.NumericUpDown universeHeightUpDown;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.Label heightLabel;
        private System.Windows.Forms.Label intervalLabel;
        private System.Windows.Forms.NumericUpDown intervalUpDown;
    }
}