namespace HoodReplace
{
    partial class ResizeTerrain
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
            this.ResizeTitle = new System.Windows.Forms.Label();
            this.Expl = new System.Windows.Forms.TextBox();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ResizePanel = new System.Windows.Forms.Panel();
            this.ResizeToFit = new System.Windows.Forms.CheckBox();
            this.ResizeGroup = new System.Windows.Forms.Panel();
            this.LeftLabel = new System.Windows.Forms.Label();
            this.TopLabel = new System.Windows.Forms.Label();
            this.LeftUpDown = new System.Windows.Forms.NumericUpDown();
            this.TopUpDown = new System.Windows.Forms.NumericUpDown();
            this.ScalePanel = new System.Windows.Forms.Panel();
            this.ScaleElevation = new System.Windows.Forms.CheckBox();
            this.ScaleGroup = new System.Windows.Forms.Panel();
            this.PercentLabel = new System.Windows.Forms.Label();
            this.ElevationUpDown = new System.Windows.Forms.NumericUpDown();
            this.ResizePanel.SuspendLayout();
            this.ResizeGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LeftUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TopUpDown)).BeginInit();
            this.ScalePanel.SuspendLayout();
            this.ScaleGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ElevationUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // ResizeTitle
            // 
            this.ResizeTitle.AutoSize = true;
            this.ResizeTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.ResizeTitle.Location = new System.Drawing.Point(5, 5);
            this.ResizeTitle.Name = "ResizeTitle";
            this.ResizeTitle.Size = new System.Drawing.Size(214, 22);
            this.ResizeTitle.TabIndex = 19;
            this.ResizeTitle.Text = "Terrain sizes are different";
            // 
            // Expl
            // 
            this.Expl.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.Expl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Expl.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Expl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Expl.Location = new System.Drawing.Point(8, 31);
            this.Expl.Multiline = true;
            this.Expl.Name = "Expl";
            this.Expl.ReadOnly = true;
            this.Expl.Size = new System.Drawing.Size(240, 72);
            this.Expl.TabIndex = 0;
            this.Expl.TabStop = false;
            this.Expl.Text = "The source terrain is smaller than the destination terrain.  Please select the st" +
                "arting coordinates for the copy.";
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(138, 160);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(50, 23);
            this.ButtonCancel.TabIndex = 10;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // ButtonOK
            // 
            this.ButtonOK.Location = new System.Drawing.Point(201, 160);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(50, 23);
            this.ButtonOK.TabIndex = 1;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // ResizePanel
            // 
            this.ResizePanel.Controls.Add(this.ScalePanel);
            this.ResizePanel.Controls.Add(this.ResizeGroup);
            this.ResizePanel.Controls.Add(this.ResizeToFit);
            this.ResizePanel.Location = new System.Drawing.Point(0, 91);
            this.ResizePanel.Name = "ResizePanel";
            this.ResizePanel.Size = new System.Drawing.Size(254, 60);
            this.ResizePanel.TabIndex = 1;
            // 
            // ResizeToFit
            // 
            this.ResizeToFit.AutoSize = true;
            this.ResizeToFit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ResizeToFit.Location = new System.Drawing.Point(9, 5);
            this.ResizeToFit.Name = "ResizeToFit";
            this.ResizeToFit.Size = new System.Drawing.Size(105, 21);
            this.ResizeToFit.TabIndex = 2;
            this.ResizeToFit.Text = "&Resize to Fit";
            this.ResizeToFit.UseVisualStyleBackColor = true;
            this.ResizeToFit.CheckedChanged += new System.EventHandler(this.ResizeToFit_CheckedChanged);
            // 
            // ResizeGroup
            // 
            this.ResizeGroup.Controls.Add(this.LeftLabel);
            this.ResizeGroup.Controls.Add(this.TopLabel);
            this.ResizeGroup.Controls.Add(this.LeftUpDown);
            this.ResizeGroup.Controls.Add(this.TopUpDown);
            this.ResizeGroup.Location = new System.Drawing.Point(131, 0);
            this.ResizeGroup.Name = "ResizeGroup";
            this.ResizeGroup.Size = new System.Drawing.Size(123, 60);
            this.ResizeGroup.TabIndex = 3;
            // 
            // LeftLabel
            // 
            this.LeftLabel.AutoSize = true;
            this.LeftLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.LeftLabel.Location = new System.Drawing.Point(4, 37);
            this.LeftLabel.Name = "LeftLabel";
            this.LeftLabel.Size = new System.Drawing.Size(32, 17);
            this.LeftLabel.TabIndex = 5;
            this.LeftLabel.Text = "&Left";
            // 
            // TopLabel
            // 
            this.TopLabel.AutoSize = true;
            this.TopLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.TopLabel.Location = new System.Drawing.Point(4, 8);
            this.TopLabel.Name = "TopLabel";
            this.TopLabel.Size = new System.Drawing.Size(33, 17);
            this.TopLabel.TabIndex = 3;
            this.TopLabel.Text = "&Top";
            // 
            // LeftUpDown
            // 
            this.LeftUpDown.Location = new System.Drawing.Point(60, 35);
            this.LeftUpDown.Name = "LeftUpDown";
            this.LeftUpDown.Size = new System.Drawing.Size(60, 20);
            this.LeftUpDown.TabIndex = 6;
            this.LeftUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TopUpDown
            // 
            this.TopUpDown.Location = new System.Drawing.Point(60, 6);
            this.TopUpDown.Name = "TopUpDown";
            this.TopUpDown.Size = new System.Drawing.Size(60, 20);
            this.TopUpDown.TabIndex = 4;
            this.TopUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ScalePanel
            // 
            this.ScalePanel.Controls.Add(this.ScaleElevation);
            this.ScalePanel.Controls.Add(this.ScaleGroup);
            this.ScalePanel.Location = new System.Drawing.Point(0, 31);
            this.ScalePanel.Name = "ScalePanel";
            this.ScalePanel.Size = new System.Drawing.Size(254, 35);
            this.ScalePanel.TabIndex = 20;
            this.ScalePanel.Visible = false;
            // 
            // ScaleElevation
            // 
            this.ScaleElevation.AutoSize = true;
            this.ScaleElevation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ScaleElevation.Location = new System.Drawing.Point(9, 5);
            this.ScaleElevation.Name = "ScaleElevation";
            this.ScaleElevation.Size = new System.Drawing.Size(124, 21);
            this.ScaleElevation.TabIndex = 6;
            this.ScaleElevation.Text = "&Scale Elevation";
            this.ScaleElevation.UseVisualStyleBackColor = true;
            this.ScaleElevation.CheckedChanged += new System.EventHandler(this.ScaleElevation_CheckedChanged);
            // 
            // ScaleGroup
            // 
            this.ScaleGroup.Controls.Add(this.PercentLabel);
            this.ScaleGroup.Controls.Add(this.ElevationUpDown);
            this.ScaleGroup.Location = new System.Drawing.Point(131, 0);
            this.ScaleGroup.Name = "ScaleGroup";
            this.ScaleGroup.Size = new System.Drawing.Size(123, 35);
            this.ScaleGroup.TabIndex = 10;
            this.ScaleGroup.Visible = false;
            // 
            // PercentLabel
            // 
            this.PercentLabel.AutoSize = true;
            this.PercentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.PercentLabel.Location = new System.Drawing.Point(4, 8);
            this.PercentLabel.Name = "PercentLabel";
            this.PercentLabel.Size = new System.Drawing.Size(57, 17);
            this.PercentLabel.TabIndex = 7;
            this.PercentLabel.Text = "&Percent";
            // 
            // ElevationUpDown
            // 
            this.ElevationUpDown.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.ElevationUpDown.Location = new System.Drawing.Point(60, 6);
            this.ElevationUpDown.Name = "ElevationUpDown";
            this.ElevationUpDown.Size = new System.Drawing.Size(60, 20);
            this.ElevationUpDown.TabIndex = 8;
            this.ElevationUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ResizeTerrain
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(260, 192);
            this.Controls.Add(this.ResizePanel);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.Expl);
            this.Controls.Add(this.ResizeTitle);
            this.MaximizeBox = false;
            this.Name = "ResizeTerrain";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Mismatched Terrain Size";
            this.Load += new System.EventHandler(this.ResizeTerrain_Load);
            this.ResizePanel.ResumeLayout(false);
            this.ResizePanel.PerformLayout();
            this.ResizeGroup.ResumeLayout(false);
            this.ResizeGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LeftUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TopUpDown)).EndInit();
            this.ScalePanel.ResumeLayout(false);
            this.ScalePanel.PerformLayout();
            this.ScaleGroup.ResumeLayout(false);
            this.ScaleGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ElevationUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ResizeTitle;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.TextBox Expl;
        private System.Windows.Forms.Panel ResizePanel;
        public System.Windows.Forms.CheckBox ResizeToFit;
        private System.Windows.Forms.Panel ResizeGroup;
        private System.Windows.Forms.Label LeftLabel;
        private System.Windows.Forms.Label TopLabel;
        public System.Windows.Forms.NumericUpDown LeftUpDown;
        public System.Windows.Forms.NumericUpDown TopUpDown;
        private System.Windows.Forms.Panel ScalePanel;
        public System.Windows.Forms.CheckBox ScaleElevation;
        private System.Windows.Forms.Panel ScaleGroup;
        private System.Windows.Forms.Label PercentLabel;
        public System.Windows.Forms.NumericUpDown ElevationUpDown;

    }
}