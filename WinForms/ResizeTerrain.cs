using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HoodReplace
{
    public partial class ResizeTerrain : Form
    {
        private string sExpl = "The terrain sizes do not match.  ";
        private string sExplIncrease = "The source terrain is smaller than the destination terrain.  ";
        private string sExplDecrease = "The source terrain is larger than the destination terrain.  ";

        private string sExplCoords = "Please select the starting coordinates for the copy.";
        private string sExplProceed = "Do you want to scale the elevation as well?";
        private string sExplScale   = "Please select the amount to scale the elevation.";

        public ResizeTerrain(int iSrcHeight, int iSrcWidth, int iDstHeight, int iDstWidth)
        {
            InitializeComponent();
            if (iSrcHeight < iDstHeight)
            {
                // Increase Size
                sExpl = sExplIncrease;
                TopUpDown.Maximum = iDstHeight - iSrcHeight;
                LeftUpDown.Maximum = iDstWidth - iSrcWidth;
                ElevationUpDown.Minimum = 101;
                ElevationUpDown.Maximum = (iDstWidth * 100) / iSrcWidth;
                ElevationUpDown.Value = ElevationUpDown.Maximum;
                if ((0 != (iDstHeight % iSrcHeight)) || (0 != (iDstWidth % iSrcWidth)))
                    ResizeToFit.Enabled = false;
            }
            else
            {
                // Decrease Size
                sExpl = sExplDecrease;
                TopUpDown.Maximum = iSrcHeight - iDstHeight;
                LeftUpDown.Maximum = iSrcWidth - iDstWidth;
                ElevationUpDown.Minimum = (iDstWidth * 100) / iSrcWidth;
                ElevationUpDown.Maximum = 99;
                ElevationUpDown.Value = ElevationUpDown.Minimum;
                if ((0 != (iSrcHeight % iDstHeight)) || (0 != (iSrcWidth % iDstWidth)))
                    ResizeToFit.Enabled = false;
            }
            TopUpDown.Value = TopUpDown.Maximum / 2;
            LeftUpDown.Value = LeftUpDown.Maximum / 2;
        }

        private void ResizeTerrain_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            Expl.Text = string.Concat(sExpl, sExplCoords);
        }

        private void ResizeToFit_CheckedChanged(object sender, EventArgs e)
        {
            if (ResizeToFit.Checked)
            {
                Expl.Text = string.Concat(sExpl, sExplProceed);
                ResizeGroup.Visible = false;
                ScalePanel.Visible = true;
            }
            else
            {
                Expl.Text = string.Concat(sExpl, sExplCoords);
                ResizeGroup.Visible = true;
                ScalePanel.Visible = false;
            }
        }

        private void ScaleElevation_CheckedChanged(object sender, EventArgs e)
        {
            if (ScaleElevation.Checked)
            {
                Expl.Text = string.Concat(sExpl, sExplScale);
                ScaleGroup.Visible = true;
            }
            else
            {
                Expl.Text = string.Concat(sExpl, sExplProceed);
                ScaleGroup.Visible = false;
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}