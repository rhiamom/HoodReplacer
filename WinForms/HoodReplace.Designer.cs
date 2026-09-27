namespace HoodReplace
{
    partial class HoodReplace
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
            this.ListSrc = new System.Windows.Forms.ListBox();
            this.LabelSrc = new System.Windows.Forms.Label();
            this.ListDst = new System.Windows.Forms.ListBox();
            this.LabelDst = new System.Windows.Forms.Label();
            this.NextButton = new System.Windows.Forms.Button();
            this.Expl = new System.Windows.Forms.TextBox();
            this.Title = new System.Windows.Forms.Label();
            this.BrowseSrc = new System.Windows.Forms.Button();
            this.BrowseDst = new System.Windows.Forms.Button();
            this.ShowEmpty = new System.Windows.Forms.CheckBox();
            this.BackButton = new System.Windows.Forms.Button();
            this.FixDeco = new System.Windows.Forms.CheckBox();
            this.FixTrees = new System.Windows.Forms.CheckBox();
            this.FixBridges = new System.Windows.Forms.CheckBox();
            this.FixRoads = new System.Windows.Forms.CheckBox();
            this.FixLots = new System.Windows.Forms.CheckBox();
            this.ReplDeco = new System.Windows.Forms.CheckBox();
            this.ReplTrees = new System.Windows.Forms.CheckBox();
            this.ReplTerrain = new System.Windows.Forms.CheckBox();
            this.ReplBridges = new System.Windows.Forms.CheckBox();
            this.ReplRoads = new System.Windows.Forms.CheckBox();
            this.DelDeco = new System.Windows.Forms.CheckBox();
            this.DelTrees = new System.Windows.Forms.CheckBox();
            this.DelBridges = new System.Windows.Forms.CheckBox();
            this.DelRoads = new System.Windows.Forms.CheckBox();
            this.MultiBackup = new System.Windows.Forms.CheckBox();
            this.OptionTable = new System.Windows.Forms.TableLayoutPanel();
            this.HeadReplace = new System.Windows.Forms.Label();
            this.HeadFix = new System.Windows.Forms.Label();
            this.HeadRemove = new System.Windows.Forms.Label();
            this.RowTerrain = new System.Windows.Forms.Label();
            this.RowRoads = new System.Windows.Forms.Label();
            this.RowBridges = new System.Windows.Forms.Label();
            this.RowTrees = new System.Windows.Forms.Label();
            this.RowDeco = new System.Windows.Forms.Label();
            this.RowLots = new System.Windows.Forms.Label();
            this.OptionTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // ListSrc
            // 
            this.ListSrc.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ListSrc.FormattingEnabled = true;
            this.ListSrc.ItemHeight = 18;
            this.ListSrc.Location = new System.Drawing.Point(7, 105);
            this.ListSrc.Name = "ListSrc";
            this.ListSrc.Size = new System.Drawing.Size(170, 220);
            this.ListSrc.TabIndex = 2;
            this.ListSrc.Tag = "0";
            this.ListSrc.SelectedIndexChanged += new System.EventHandler(this.ListSrc_SelectedIndexChanged);
            this.ListSrc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListSrc_KeyDown);
            // 
            // LabelSrc
            // 
            this.LabelSrc.AutoSize = true;
            this.LabelSrc.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSrc.Location = new System.Drawing.Point(4, 78);
            this.LabelSrc.Name = "LabelSrc";
            this.LabelSrc.Size = new System.Drawing.Size(90, 20);
            this.LabelSrc.TabIndex = 1;
            this.LabelSrc.Text = "Copy &From:";
            // 
            // ListDst
            // 
            this.ListDst.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.ListDst.FormattingEnabled = true;
            this.ListDst.ItemHeight = 18;
            this.ListDst.Location = new System.Drawing.Point(188, 105);
            this.ListDst.Name = "ListDst";
            this.ListDst.Size = new System.Drawing.Size(170, 220);
            this.ListDst.TabIndex = 5;
            this.ListDst.Tag = "0";
            this.ListDst.SelectedIndexChanged += new System.EventHandler(this.ListDst_SelectedIndexChanged);
            this.ListDst.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListDst_KeyDown);
            // 
            // LabelDst
            // 
            this.LabelDst.AutoSize = true;
            this.LabelDst.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelDst.Location = new System.Drawing.Point(186, 78);
            this.LabelDst.Name = "LabelDst";
            this.LabelDst.Size = new System.Drawing.Size(71, 20);
            this.LabelDst.TabIndex = 4;
            this.LabelDst.Text = "Copy T&o:";
            // 
            // NextButton
            // 
            this.NextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NextButton.Location = new System.Drawing.Point(695, 329);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(55, 23);
            this.NextButton.TabIndex = 18;
            this.NextButton.Text = "Copy";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // Expl
            // 
            this.Expl.BackColor = System.Drawing.SystemColors.Control;
            this.Expl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Expl.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Expl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Expl.Location = new System.Drawing.Point(8, 31);
            this.Expl.Multiline = true;
            this.Expl.Name = "Expl";
            this.Expl.ReadOnly = true;
            this.Expl.Size = new System.Drawing.Size(742, 40);
            this.Expl.TabIndex = 0;
            this.Expl.TabStop = false;
            this.Expl.Text = "This program will copy terrain and structures from one Sims 2 neighborhood to ano" +
                "ther, completely replacing the original terrain and structures.";
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.Title.Location = new System.Drawing.Point(5, 5);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(257, 22);
            this.Title.TabIndex = 8;
            this.Title.Text = "Replace Neighborhood Terrain";
            // 
            // BrowseSrc
            // 
            this.BrowseSrc.Location = new System.Drawing.Point(127, 78);
            this.BrowseSrc.Name = "BrowseSrc";
            this.BrowseSrc.Size = new System.Drawing.Size(50, 23);
            this.BrowseSrc.TabIndex = 3;
            this.BrowseSrc.Text = "Bro&wse";
            this.BrowseSrc.UseVisualStyleBackColor = true;
            this.BrowseSrc.Click += new System.EventHandler(this.BrowseSrc_Click);
            // 
            // BrowseDst
            // 
            this.BrowseDst.Location = new System.Drawing.Point(308, 78);
            this.BrowseDst.Name = "BrowseDst";
            this.BrowseDst.Size = new System.Drawing.Size(50, 23);
            this.BrowseDst.TabIndex = 6;
            this.BrowseDst.Text = "Brow&se";
            this.BrowseDst.UseVisualStyleBackColor = true;
            this.BrowseDst.Click += new System.EventHandler(this.BrowseDst_Click);
            // 
            // ShowEmpty
            // 
            this.ShowEmpty.AutoSize = true;
            this.ShowEmpty.Checked = true;
            this.ShowEmpty.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowEmpty.Location = new System.Drawing.Point(7, 335);
            this.ShowEmpty.Name = "ShowEmpty";
            this.ShowEmpty.Size = new System.Drawing.Size(160, 17);
            this.ShowEmpty.TabIndex = 20;
            this.ShowEmpty.Text = "Show Empty &Neighborhoods";
            this.ShowEmpty.UseVisualStyleBackColor = true;
            this.ShowEmpty.CheckedChanged += new System.EventHandler(this.ShowEmpty_CheckedChanged);
            // 
            // BackButton
            // 
            this.BackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BackButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BackButton.Location = new System.Drawing.Point(627, 329);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(55, 23);
            this.BackButton.TabIndex = 19;
            this.BackButton.Text = "Exit";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // 
            // FixDeco
            // 
            this.FixDeco.Name = "FixDeco";
            this.FixDeco.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FixDeco.AutoSize = true;
            this.FixDeco.TabIndex = 36;
            this.FixDeco.UseVisualStyleBackColor = true;
            // 
            // FixTrees
            // 
            this.FixTrees.Name = "FixTrees";
            this.FixTrees.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FixTrees.AutoSize = true;
            this.FixTrees.TabIndex = 35;
            this.FixTrees.UseVisualStyleBackColor = true;
            // 
            // FixBridges
            // 
            this.FixBridges.Enabled = false;
            this.FixBridges.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FixBridges.AutoSize = true;
            this.FixBridges.Name = "FixBridges";
            this.FixBridges.TabIndex = 34;
            this.FixBridges.UseVisualStyleBackColor = true;
            // 
            // FixRoads
            // 
            this.FixRoads.Enabled = false;
            this.FixRoads.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FixRoads.AutoSize = true;
            this.FixRoads.Name = "FixRoads";
            this.FixRoads.TabIndex = 33;
            this.FixRoads.UseVisualStyleBackColor = true;
            // 
            // FixLots
            // 
            this.FixLots.Checked = true;
            this.FixLots.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FixLots.AutoSize = true;
            this.FixLots.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FixLots.Name = "FixLots";
            this.FixLots.TabIndex = 29;
            this.FixLots.Text = "";
            this.FixLots.UseVisualStyleBackColor = true;
            // 
            // 
            // ReplDeco
            // 
            this.ReplDeco.Checked = true;
            this.ReplDeco.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ReplDeco.AutoSize = true;
            this.ReplDeco.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ReplDeco.Name = "ReplDeco";
            this.ReplDeco.TabIndex = 16;
            this.ReplDeco.Text = "";
            this.ReplDeco.UseVisualStyleBackColor = true;
            this.ReplDeco.CheckedChanged += new System.EventHandler(this.ReplDeco_CheckedChanged);
            // 
            // ReplTrees
            // 
            this.ReplTrees.Checked = true;
            this.ReplTrees.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ReplTrees.AutoSize = true;
            this.ReplTrees.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ReplTrees.Name = "ReplTrees";
            this.ReplTrees.TabIndex = 15;
            this.ReplTrees.Text = "";
            this.ReplTrees.UseVisualStyleBackColor = true;
            this.ReplTrees.CheckedChanged += new System.EventHandler(this.ReplTrees_CheckedChanged);
            // 
            // ReplTerrain
            // 
            this.ReplTerrain.Checked = true;
            this.ReplTerrain.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ReplTerrain.AutoSize = true;
            this.ReplTerrain.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ReplTerrain.Name = "ReplTerrain";
            this.ReplTerrain.TabIndex = 12;
            this.ReplTerrain.Text = "";
            this.ReplTerrain.UseVisualStyleBackColor = true;
            // 
            // ReplBridges
            // 
            this.ReplBridges.Checked = true;
            this.ReplBridges.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ReplBridges.AutoSize = true;
            this.ReplBridges.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ReplBridges.Name = "ReplBridges";
            this.ReplBridges.TabIndex = 14;
            this.ReplBridges.Text = "";
            this.ReplBridges.UseVisualStyleBackColor = true;
            this.ReplBridges.CheckedChanged += new System.EventHandler(this.ReplBridges_CheckedChanged);
            // 
            // ReplRoads
            // 
            this.ReplRoads.Checked = true;
            this.ReplRoads.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ReplRoads.AutoSize = true;
            this.ReplRoads.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ReplRoads.Name = "ReplRoads";
            this.ReplRoads.TabIndex = 13;
            this.ReplRoads.Text = "";
            this.ReplRoads.UseVisualStyleBackColor = true;
            this.ReplRoads.CheckedChanged += new System.EventHandler(this.ReplRoads_CheckedChanged);
            // 
            // 
            // DelDeco
            // 
            this.DelDeco.Name = "DelDeco";
            this.DelDeco.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DelDeco.AutoSize = true;
            this.DelDeco.TabIndex = 30;
            this.DelDeco.UseVisualStyleBackColor = true;
            this.DelDeco.CheckedChanged += new System.EventHandler(this.DelDeco_CheckedChanged);
            // 
            // DelTrees
            // 
            this.DelTrees.Name = "DelTrees";
            this.DelTrees.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DelTrees.AutoSize = true;
            this.DelTrees.TabIndex = 29;
            this.DelTrees.UseVisualStyleBackColor = true;
            this.DelTrees.CheckedChanged += new System.EventHandler(this.DelTrees_CheckedChanged);
            // 
            // DelBridges
            // 
            this.DelBridges.Name = "DelBridges";
            this.DelBridges.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DelBridges.AutoSize = true;
            this.DelBridges.TabIndex = 28;
            this.DelBridges.UseVisualStyleBackColor = true;
            this.DelBridges.CheckedChanged += new System.EventHandler(this.DelBridges_CheckedChanged);
            // 
            // DelRoads
            // 
            this.DelRoads.Name = "DelRoads";
            this.DelRoads.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DelRoads.AutoSize = true;
            this.DelRoads.TabIndex = 27;
            this.DelRoads.UseVisualStyleBackColor = true;
            this.DelRoads.CheckedChanged += new System.EventHandler(this.DelRoads_CheckedChanged);
            // 
            // MultiBackup
            // 
            this.MultiBackup.AutoSize = true;
            this.MultiBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MultiBackup.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.MultiBackup.Location = new System.Drawing.Point(188, 335);
            this.MultiBackup.Name = "MultiBackup";
            this.MultiBackup.Size = new System.Drawing.Size(118, 17);
            this.MultiBackup.TabIndex = 40;
            this.MultiBackup.Text = "&Versioned Backups";
            this.MultiBackup.UseVisualStyleBackColor = true;
            this.MultiBackup.CheckedChanged += new System.EventHandler(this.MultiBackup_CheckedChanged);
            // 
            // HeadReplace
            // 
            this.HeadReplace.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.HeadReplace.AutoSize = true;
            this.HeadReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.HeadReplace.Name = "HeadReplace";
            this.HeadReplace.Text = "Replace";
            // 
            // HeadFix
            // 
            this.HeadFix.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.HeadFix.AutoSize = true;
            this.HeadFix.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.HeadFix.Name = "HeadFix";
            this.HeadFix.Text = "Fix Elevation";
            // 
            // HeadRemove
            // 
            this.HeadRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.HeadRemove.AutoSize = true;
            this.HeadRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.HeadRemove.Name = "HeadRemove";
            this.HeadRemove.Text = "Remove";
            // 
            // RowTerrain
            // 
            this.RowTerrain.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.RowTerrain.AutoSize = true;
            this.RowTerrain.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.RowTerrain.Name = "RowTerrain";
            this.RowTerrain.Text = "&Terrain";
            // 
            // RowRoads
            // 
            this.RowRoads.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.RowRoads.AutoSize = true;
            this.RowRoads.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.RowRoads.Name = "RowRoads";
            this.RowRoads.Text = "&Roads";
            // 
            // RowBridges
            // 
            this.RowBridges.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.RowBridges.AutoSize = true;
            this.RowBridges.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.RowBridges.Name = "RowBridges";
            this.RowBridges.Text = "&Bridges";
            // 
            // RowTrees
            // 
            this.RowTrees.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.RowTrees.AutoSize = true;
            this.RowTrees.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.RowTrees.Name = "RowTrees";
            this.RowTrees.Text = "Tr&ees";
            // 
            // RowDeco
            // 
            this.RowDeco.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.RowDeco.AutoSize = true;
            this.RowDeco.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.RowDeco.Name = "RowDeco";
            this.RowDeco.Text = "&Decorations";
            // 
            // RowLots
            // 
            this.RowLots.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.RowLots.AutoSize = true;
            this.RowLots.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.RowLots.Name = "RowLots";
            this.RowLots.Text = "&Lots             ";
            // 
            // OptionTable
            // 
            this.OptionTable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OptionTable.ColumnCount = 4;
            this.OptionTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 114F));
            this.OptionTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.OptionTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.OptionTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.OptionTable.Controls.Add(this.HeadReplace, 1, 0);
            this.OptionTable.Controls.Add(this.HeadFix, 2, 0);
            this.OptionTable.Controls.Add(this.HeadRemove, 3, 0);
            this.OptionTable.Controls.Add(this.RowTerrain, 0, 1);
            this.OptionTable.Controls.Add(this.RowRoads, 0, 2);
            this.OptionTable.Controls.Add(this.RowBridges, 0, 3);
            this.OptionTable.Controls.Add(this.RowTrees, 0, 4);
            this.OptionTable.Controls.Add(this.RowDeco, 0, 5);
            this.OptionTable.Controls.Add(this.RowLots, 0, 6);
            this.OptionTable.Controls.Add(this.ReplTerrain, 1, 1);
            this.OptionTable.Controls.Add(this.ReplRoads, 1, 2);
            this.OptionTable.Controls.Add(this.ReplBridges, 1, 3);
            this.OptionTable.Controls.Add(this.ReplTrees, 1, 4);
            this.OptionTable.Controls.Add(this.ReplDeco, 1, 5);
            this.OptionTable.Controls.Add(this.FixRoads, 2, 2);
            this.OptionTable.Controls.Add(this.FixBridges, 2, 3);
            this.OptionTable.Controls.Add(this.FixTrees, 2, 4);
            this.OptionTable.Controls.Add(this.FixDeco, 2, 5);
            this.OptionTable.Controls.Add(this.FixLots, 2, 6);
            this.OptionTable.Controls.Add(this.DelRoads, 3, 2);
            this.OptionTable.Controls.Add(this.DelBridges, 3, 3);
            this.OptionTable.Controls.Add(this.DelTrees, 3, 4);
            this.OptionTable.Controls.Add(this.DelDeco, 3, 5);
            this.OptionTable.Location = new System.Drawing.Point(368, 105);
            this.OptionTable.Name = "OptionTable";
            this.OptionTable.RowCount = 7;
            this.OptionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.OptionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.OptionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.OptionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.OptionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.OptionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.OptionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.OptionTable.Size = new System.Drawing.Size(384, 220);
            this.OptionTable.TabIndex = 38;
            // 
            // HoodReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 358);
            this.Controls.Add(this.OptionTable);
            this.Controls.Add(this.MultiBackup);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.ShowEmpty);
            this.Controls.Add(this.BrowseDst);
            this.Controls.Add(this.BrowseSrc);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.Expl);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.LabelDst);
            this.Controls.Add(this.ListDst);
            this.Controls.Add(this.LabelSrc);
            this.Controls.Add(this.ListSrc);
            this.Name = "HoodReplace";
            this.Text = "HoodReplace 2.2";
            this.Shown += new System.EventHandler(this.HoodReplace_Shown);
            this.Load += new System.EventHandler(this.HoodReplace_Load);
            this.OptionTable.ResumeLayout(false);
            this.OptionTable.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ListSrc;
        private System.Windows.Forms.Label LabelSrc;
        private System.Windows.Forms.ListBox ListDst;
        private System.Windows.Forms.Label LabelDst;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.TextBox Expl;
        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Button BrowseSrc;
        private System.Windows.Forms.Button BrowseDst;
        private System.Windows.Forms.CheckBox ShowEmpty;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.CheckBox FixDeco;
        private System.Windows.Forms.CheckBox FixTrees;
        private System.Windows.Forms.CheckBox FixBridges;
        private System.Windows.Forms.CheckBox FixRoads;
        private System.Windows.Forms.CheckBox FixLots;
        private System.Windows.Forms.CheckBox ReplDeco;
        private System.Windows.Forms.CheckBox ReplTrees;
        private System.Windows.Forms.CheckBox ReplTerrain;
        private System.Windows.Forms.CheckBox ReplBridges;
        private System.Windows.Forms.CheckBox ReplRoads;
        private System.Windows.Forms.CheckBox DelDeco;
        private System.Windows.Forms.CheckBox DelTrees;
        private System.Windows.Forms.CheckBox DelBridges;
        private System.Windows.Forms.CheckBox DelRoads;
        private System.Windows.Forms.CheckBox MultiBackup;
        private System.Windows.Forms.TableLayoutPanel OptionTable;
        private System.Windows.Forms.Label HeadReplace;
        private System.Windows.Forms.Label HeadFix;
        private System.Windows.Forms.Label HeadRemove;
        private System.Windows.Forms.Label RowTerrain;
        private System.Windows.Forms.Label RowRoads;
        private System.Windows.Forms.Label RowBridges;
        private System.Windows.Forms.Label RowTrees;
        private System.Windows.Forms.Label RowDeco;
        private System.Windows.Forms.Label RowLots;
    }
}

