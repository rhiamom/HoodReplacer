/***************************************************************************
 *   Copyright (C) 2008-2010 by Mootilda                                   *
 *   http://Mootilda.ModTheSims.info                                       *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.             *
 ***************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SimPe.Interfaces.Files;
using SimPe.Packages;
using System.Resources;


namespace HoodReplace
{
    public partial class HoodReplace : Form
    {
        private bool Test_PrintDebugInfo = false;   // Enable (T) or disable (F) printing of debug information

        private bool bBackupVersioning = false;     // Keep multiple versions of backup files
        private string sBackupConfigFile = null;

        private bool bSrcArrowUp = false;
        private bool bDstArrowUp = false;

        private char[] spaces = { ' ' };
        private string sSrcNbhdName;
        private string sDstNbhdName;
        private string sSrcTrimName;
        private string sDstTrimName;

        private bool bResizeArrays = false;
        private bool bScaleArrays = false;

        int iSrcTop = 0;
        int iSrcLeft = 0;
        int iSrcHeight = 0;
        int iSrcWidth = 0;

        int iDstTop = 0;
        int iDstLeft = 0;
        int iDstHeight = 0;
        int iDstWidth = 0;

        private float[,] fTerrain = null;

        private const string sSC4Ext = ".SC4";
        private bool bSC4Src = false;
        private bool bSC4Dst = false;
        private const float SC4Adjustment = 63.5F;  // Water level TS2 = 312.5; SC4 ~ 249
        private const float TS2WaterLevel = 312.5F;

        public HoodReplace()
        {
            InitializeComponent();
        }

        private void HoodReplace_Load(object sender, EventArgs e)
        {
            try
            {
                sBackupConfigFile = Application.ExecutablePath;
                sBackupConfigFile = Path.GetDirectoryName(sBackupConfigFile);
                sBackupConfigFile = string.Concat(sBackupConfigFile, Path.DirectorySeparatorChar, "HRBKPVER.TXT");
                bBackupVersioning = System.IO.File.Exists(sBackupConfigFile);
            }
            catch
            {
            }
            NeighborhoodScreen();
            // this.CenterToScreen();
        }

        private void HoodReplace_Shown(object sender, EventArgs e)
        {
            MultiBackup.Checked = bBackupVersioning;
            NextButton.Focus();
        }

        private void ShowEmpty_CheckedChanged(object sender, EventArgs e)
        {
            NeighborhoodScreen();
        }

        private string GetPath()
        {
            string sPath;
            string sMyDocs = null;
            string sEAGames = null;
            string sSims2 = null;
            string sNbhds = null;
            try
            {
                sMyDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (Test_PrintDebugInfo)
                    Debug.Print("My Documents:  {0}", sMyDocs);
                sEAGames = Path.Combine(sMyDocs, "EA Games");
                if (Test_PrintDebugInfo)
                    Debug.Print("EA Games:      {0}", sEAGames);
                sSims2 = Path.Combine(sEAGames,
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\EA Games\\The Sims 2").GetValue("DisplayName").ToString());
                if (Test_PrintDebugInfo)
                    Debug.Print("The Sims 2:    {0}", sSims2);
                sNbhds = Path.Combine(sSims2, "Neighborhoods");
                if (Test_PrintDebugInfo)
                    Debug.Print("Neighborhoods: {0}", sNbhds);
            }
            catch
            {
            }
            sPath = sNbhds;
            if ((null == sPath) || !System.IO.Directory.Exists(sPath))
                sPath = sSims2;
            if ((null == sPath) || !System.IO.Directory.Exists(sPath))
                sPath = sEAGames;
            if ((null == sPath) || !System.IO.Directory.Exists(sPath))
                sPath = sMyDocs;

            return sPath;
        }

        // Find the name of a loaded neighborhood
        private string HoodName(ref GeneratableFile NBPack)
        {
            IPackedFileDescriptor NBDescription = NBPack.FindFile(0x43545353, 0, 0xFFFFFFFF, 1);
            IPackedFile PF = NBPack.Read(NBDescription);
            int z = 0;
            while (PF.UncompressedData[69 + z++] != 0)
            {
            }
            byte[] BD = new byte[--z];
            Array.Copy(PF.UncompressedData, 69, BD, 0, z);
            string NBName = SimPe.Helper.ToString(BD);
            NBName.Replace(":", ";");
            return NBName;
        }

        // Load a neighborhood to find the name
        private string LoadHoodName(string sNeighborhoodPath, bool bSkipEmpty)
        {
            string NBName = null;
            DialogResult dResult = DialogResult.OK;
            do
            {
                dResult = DialogResult.OK;
                try
                {
                    GeneratableFile NBPack = SimPe.Packages.File.LoadFromFile(sNeighborhoodPath);
                    if (bSkipEmpty)
                    {
                        IPackedFileDescriptor[] LotDesc = NBPack.FindFiles(0x0BF999E7);
                        // Skip hidden neighborhoods, like Pets, Weather (Seasons), and Exotic Destinations (Bon Voyage)
                        if (0 == LotDesc.Length)
                            continue;
                    }
                    if (sSC4Ext == Path.GetExtension(sNeighborhoodPath).ToUpper())
                        NBName = Path.GetFileName(sNeighborhoodPath);
                    else
                        NBName = HoodName(ref NBPack);
                }
                catch (Exception e)
                {
                    NBName = null;
                    dResult = MessageBox.Show(
                        String.Format("Cannot open neighborhood:\n{0}\n\n{1}", sNeighborhoodPath, e.Message),
                        "Fatal Error!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                    if (DialogResult.Abort == dResult)
                    {
                        this.Close();
                        return null;
                    }
                }
            }
            while (dResult == DialogResult.Retry);
            return NBName;
        }

        private void NeighborhoodScreen()
        {
            // string[] dirs = { "E*", "F*", "G*", "N*" };
            string[] dirs = { "*" };
            string sPath = GetPath();

            ListSrc.Tag = 0;
            ListSrc.BeginUpdate();
            ListSrc.Items.Clear();
            ListSrc.Sorted = false;

            ListDst.Tag = 0;
            ListDst.BeginUpdate();
            ListDst.Items.Clear();
            ListDst.Sorted = false;

            System.Collections.ArrayList FileListSrc = new System.Collections.ArrayList();
            System.Collections.ArrayList FileListDst = new System.Collections.ArrayList();
            for (int h = 0; h < dirs.Length; h++)
            {
                string[] NBOrdner = Directory.GetDirectories(sPath, dirs[h]);
                Array.Sort(NBOrdner);
                for (int i = 0; i < NBOrdner.Length; i++)
                {
                    string sHoodName = Path.GetFileName(NBOrdner[i]);
                    if (0 == string.Compare("Tutorial", sHoodName))
                        continue;
                    if (FileListSrc.Count > 0)
                    {
                        ListSrc.Items.Add("");
                        FileListSrc.Add("");
                    }
                    if (FileListDst.Count > 0)
                    {
                        ListDst.Items.Add("");
                        FileListDst.Add("");
                    }
                    ListSrc.Items.Add(sHoodName + ":");
                    FileListSrc.Add("");
                    ListDst.Items.Add(sHoodName + ":");
                    FileListDst.Add("");

                    // List main neighborhood first
                    string[] MainNeighborhood = Directory.GetFiles(NBOrdner[i], Path.GetFileName(NBOrdner[i]) + "_Neighborhood.package");
                    Debug.Assert(MainNeighborhood.Length < 2);
                    if (1 == MainNeighborhood.Length)
                    {
                        try
                        {
                            string NBName = LoadHoodName(MainNeighborhood[0], false);
                            if (null != NBName)
                            {
                                ListSrc.Items.Add("    " + NBName);
                                FileListSrc.Add(MainNeighborhood[0]);
                                ListDst.Items.Add("    " + NBName);
                                FileListDst.Add(MainNeighborhood[0]);
                                // Print a list of neighborhood names for debugging purposes:
                                if (Test_PrintDebugInfo)
                                    Debug.Print("{0}: {1}", Path.GetFileName(NBOrdner[i]), NBName);
                            }
                        }
                        catch
                        {
                        }
                    }

                    // Then list subneighborhoods
                    string[] AlleNBinOrdner = Directory.GetFiles(NBOrdner[i], "*.package");
                    for (int j = 0; j < AlleNBinOrdner.Length; j++)
                    {
                        // We've already done the main neighborhood...
                        if ((1 == MainNeighborhood.Length) && (MainNeighborhood[0] == AlleNBinOrdner[j]))
                            continue;
                        try
                        {
                            string NBName = LoadHoodName(AlleNBinOrdner[j], !ShowEmpty.Checked);
                            if (null != NBName)
                            {
                                ListSrc.Items.Add("    " + NBName);
                                FileListSrc.Add(AlleNBinOrdner[j]);
                                ListDst.Items.Add("    " + NBName);
                                FileListDst.Add(AlleNBinOrdner[j]);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            ListSrc.EndUpdate();
            ListSrc.Tag = FileListSrc;
            ListSrc.Visible = true;

            ListDst.EndUpdate();
            ListDst.Tag = FileListDst;
            ListDst.Visible = true;

            NextButton.Enabled = false;
            if (0 != ListSrc.Items.Count)
            {
                ListSrc.SelectedIndex = 1;
                ListSrc.Focus();
                ListDst.SelectedIndex = 1;
            }
        }

        private void BrowseSrc_Click(object sender, EventArgs e)
        {
            BrowseButton_Click(ListSrc, true);
        }

        private void BrowseDst_Click(object sender, EventArgs e)
        {
            BrowseButton_Click(ListDst, false);
        }

        private void BrowseButton_Click(ListBox lb, bool bAllowSC4)
        {
            DialogResult res = DialogResult.OK;
            GeneratableFile NBPack = null;
            OpenFileDialog BrowseDialog = new OpenFileDialog();
            string sNeighborhoodPath = "";
            bool bSC4File = false;

            System.Collections.ArrayList fl = (System.Collections.ArrayList)lb.Tag;
            Debug.Assert(fl != null);
            while (res == DialogResult.OK)
            {
                BrowseDialog.Filter =
                    "Sims2 Neighborhood files (*.package))|*_Neighborhood.package;*_Downtown*.package;*_Suburb*.package;*_University*.package;*_Vacation*.package";
                if (bAllowSC4)
                    BrowseDialog.Filter = string.Concat( BrowseDialog.Filter, "|SimCity4 files (*.SC4))|*.SC4");
                BrowseDialog.FilterIndex = 0;
                BrowseDialog.RestoreDirectory = false;
                if (bAllowSC4)
                    BrowseDialog.Title = "Choose a Neighbourhood or SimCity4 file:";
                else
                    BrowseDialog.Title = "Choose a Neighbourhood:";
                res = BrowseDialog.ShowDialog();
                if (DialogResult.OK == res)
                {
                    sNeighborhoodPath = BrowseDialog.FileName;
                    if (sSC4Ext == Path.GetExtension(sNeighborhoodPath).ToUpper())
                        bSC4File = true;
                    else
                        bSC4File = false;
                    NBPack = SimPe.Packages.File.LoadFromFile(sNeighborhoodPath);
                    if (null != NBPack)
                        break;
                    if (bSC4File)
                        MessageBox.Show("Unable to open SimCity4 file", "Error");
                    else
                        MessageBox.Show("Unable to open neighborhood", "Error");
                }
            }
            if ((DialogResult.OK == res) && (null != NBPack))
            {
                // Display neighborhood which is not in the list.
                lb.BeginUpdate();
                lb.Items.Clear();
                fl.Clear();
                lb.Sorted = false;

                if (bSC4File)
                    // ToDo: Open SimCity file and find city name?
                    lb.Items.Add("SimCity4 File:");
                else
                    lb.Items.Add("Special:");
                fl.Add("");
                string NBName;
                if (bSC4File)
                    NBName = Path.GetFileName(sNeighborhoodPath);
                else
                    NBName = HoodName(ref NBPack);
                lb.Items.Add("    " + NBName);
                fl.Add(sNeighborhoodPath);

                lb.EndUpdate();
                lb.Tag = fl;
                lb.Visible = true;
                lb.SelectedIndex = 1;
            }
        }

        private bool ReadyToReplace(bool bFixCheckBoxes)
        {
            if ((-1 == ListSrc.SelectedIndex) || (-1 == ListDst.SelectedIndex))
                return false;

            System.Collections.ArrayList FileNameSrc = (System.Collections.ArrayList)ListSrc.Tag;
            if (null == FileNameSrc)
                return false;
            string sSrcPackName = FileNameSrc[ListSrc.SelectedIndex].ToString();

            System.Collections.ArrayList FileNameDst = (System.Collections.ArrayList)ListDst.Tag;
            if (null == FileNameDst)
                return false;
            string sDstPackName = FileNameDst[ListDst.SelectedIndex].ToString();

            if (sSrcPackName == sDstPackName)
                return false;

            bSC4Src = (Path.GetExtension(sSrcPackName).ToUpper() == sSC4Ext);
            bSC4Dst = (Path.GetExtension(sDstPackName).ToUpper() == sSC4Ext);
            if (bSC4Src && bSC4Dst) 
                return false;
            if (bFixCheckBoxes) // Don't change the checkboxes if we're actually ready to do the copy.
            {
                if (bSC4Src || bSC4Dst)
                {
                    ReplRoads.Enabled = ReplRoads.Checked = false;
                    ReplBridges.Enabled = ReplBridges.Checked = false;
                    ReplTrees.Enabled = ReplTrees.Checked = false;
                    ReplDeco.Enabled = ReplDeco.Checked = false;
                }
                else
                {
                    ReplRoads.Enabled = ReplRoads.Checked = true;
                    ReplBridges.Enabled = ReplBridges.Checked = true;
                    ReplTrees.Enabled = ReplTrees.Checked = true;
                    ReplDeco.Enabled = ReplDeco.Checked = true;
                    FixTrees.Checked = FixDeco.Checked = false;
                }
            }

            if (ReplTerrain.Checked
             || ReplRoads.Checked || ReplBridges.Checked || ReplTrees.Checked || ReplDeco.Checked
             || FixRoads.Checked || FixBridges.Checked || FixTrees.Checked || FixDeco.Checked || FixLots.Checked
             || DelRoads.Checked || DelBridges.Checked || DelTrees.Checked || DelDeco.Checked)
                return true;
            return false;
        }

        private void CheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            NextButton.Enabled = ReadyToReplace(true);
        }

        private void ListSrc_KeyDown(object sender, KeyEventArgs e)
        {
            if ((0x26 == e.KeyValue) && (ListSrc.SelectedIndex > 1))
                bSrcArrowUp = true;
        }

        private void ListSrc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (-1 != ListSrc.SelectedIndex)
            {
                NextButton.Enabled = ReadyToReplace(true);
                string s = ListSrc.SelectedItem.ToString();
                if ((0 == string.Compare(s, "")) || (-1 != s.IndexOf(":", 0)))
                {
                    if (bSrcArrowUp)
                        ListSrc.SelectedIndex -= 1;
                    else if ((ListSrc.SelectedIndex + 1) < ListSrc.Items.Count)
                        ListSrc.SelectedIndex += 1;
                }
            }
            else
            {
                NextButton.Enabled = false;
            }
            bSrcArrowUp = false;
        }

        private void ListDst_KeyDown(object sender, KeyEventArgs e)
        {
            if ((0x26 == e.KeyValue) && (ListDst.SelectedIndex > 1))
                bDstArrowUp = true;
        }

        private void ListDst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (-1 != ListDst.SelectedIndex)
            {
                NextButton.Enabled = ReadyToReplace(true);
                string s = ListDst.SelectedItem.ToString();
                if ((0 == string.Compare(s, "")) || (-1 != s.IndexOf(":", 0)))
                {
                    if (bDstArrowUp)
                        ListDst.SelectedIndex -= 1;
                    else if ((ListDst.SelectedIndex + 1) < ListDst.Items.Count)
                        ListDst.SelectedIndex += 1;
                }
            }
            else
            {
                NextButton.Enabled = false;
            }
            bDstArrowUp = false;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            bool bChanged = false;

            this.Cursor = Cursors.WaitCursor;
            if (ReadyToReplace(false))
                bChanged = ReplaceFile();
            this.Cursor = Cursors.Default;
            if (null != sBackupConfigFile)
            {
                try
                {
                    if (!bBackupVersioning)
                        System.IO.File.Delete(sBackupConfigFile);
                    else if (!System.IO.File.Exists(sBackupConfigFile))
                        System.IO.File.Create(sBackupConfigFile);
                }
                catch
                {
                }
            }
            this.Close();
            // Application.Exit();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ReplaceFile()
        {
            // Open the source neighborhood:
            GeneratableFile NBPackSrc;
            sSrcNbhdName = ListSrc.SelectedItem.ToString();
            sSrcTrimName = sSrcNbhdName.TrimStart(spaces);
            if (sSrcTrimName.Contains(" "))
                sSrcTrimName = string.Concat(string.Concat("\"", sSrcTrimName), "\"");
            try
            {
                System.Collections.ArrayList FileNames = (System.Collections.ArrayList)ListSrc.Tag;
                string sSrcPackName = FileNames[ListSrc.SelectedIndex].ToString();
                NBPackSrc = SimPe.Packages.File.LoadFromFile(sSrcPackName);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    String.Format("Cannot open source neighborhood:\n{0}\n\n{1}", sSrcNbhdName, e.Message),
                    "Copy Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Open the destination neighborhood:
            GeneratableFile NBPackDst;
            sDstNbhdName = ListDst.SelectedItem.ToString();
            sDstTrimName = sDstNbhdName.TrimStart(spaces);
            if (sDstTrimName.Contains(" "))
                sDstTrimName = string.Concat(string.Concat("\"", sDstTrimName), "\"");
            try
            {
                System.Collections.ArrayList FileNames = (System.Collections.ArrayList)ListDst.Tag;
                string sDstPackName = FileNames[ListDst.SelectedIndex].ToString();
                NBPackDst = SimPe.Packages.File.LoadFromFile(sDstPackName);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    String.Format("Cannot open destination neighborhood:\n{0}\n\n{1}", sDstNbhdName, e.Message),
                    "Copy Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NBPackSrc.ForgetUpdate();
                NBPackSrc.Close();
                return false;
            }

            // Make the specified changes:
            bool bChanged = ReplaceRecords(ref NBPackSrc, ref NBPackDst);

            // Probably unnecessary, but just to be safe, abort any changes to source neighborhood
            NBPackSrc.ForgetUpdate();
            NBPackSrc.Close();

            // Save or abort destination neighborhood changes
            if (bChanged)
            {
                // ToDo: check for errors?
                PackageSave(NBPackDst);

                MessageBox.Show(String.Format("Copied from {0} to {1}.", sSrcTrimName, sDstTrimName),
                    "Copy Complete.", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            else
                NBPackDst.ForgetUpdate();
            NBPackDst.Close();

            return bChanged;
        }

        private bool ReplaceRecords(ref GeneratableFile NBPackSrc, ref GeneratableFile NBPackDst)
        {
            R_Terrain rSrc = SrcTerrain(ref NBPackSrc);
            if (null == rSrc)
                return false;

            R_Terrain rDst = DstTerrain(ref NBPackDst);
            if (null == rDst)
                return false;

            iSrcTop = 0;
            iSrcLeft = 0;
            iSrcHeight = rSrc.Height;
            iSrcWidth = rSrc.Width;

            iDstTop = 0;
            iDstLeft = 0;
            iDstHeight = rDst.Height;
            iDstWidth = rDst.Width;

            try
            {
                if (ReplTerrain.Checked)
                {
                    if (!ReplaceNHTG(ref rSrc, ref rDst))
                        return false;
                }
                if (ReplRoads.Checked || ReplBridges.Checked || ReplTrees.Checked || ReplDeco.Checked
                 || FixRoads.Checked || FixBridges.Checked || FixTrees.Checked || FixDeco.Checked || FixLots.Checked
                 || DelRoads.Checked || DelBridges.Checked || DelTrees.Checked || DelDeco.Checked)
                {
                    if ((fTerrain == null)
                     && (FixRoads.Checked || FixBridges.Checked || FixTrees.Checked || FixDeco.Checked || FixLots.Checked))
                    {
                        if (ReplTerrain.Checked)
                        {
                            // If (ReplTerrain.Checked) && (fTerrain == null),
                            //   then we must have copied the raw terrain data
                            // When we change the raw terrain data,
                            //   rDst.GetTerrain gets the old terrain instead of the new one
                            //   so use the source terrain instead
                            fTerrain = rSrc.GetTerrain(iSrcTop, iSrcLeft, iSrcHeight, iSrcWidth);
                        }
                        else
                            fTerrain = rDst.GetTerrain(iDstTop, iDstLeft, iDstHeight, iDstWidth);
                    }
                    if (!ReplaceNHTR(ref NBPackSrc, ref NBPackDst))
                        return false;
                }
                if (FixLots.Checked)
                {
                    const float iSpaceAboveTerrain = .1F;

                    IPackedFileDescriptor[] LotDescription = NBPackDst.FindFiles(0x0BF999E7);
                    foreach (IPackedFileDescriptor LotDescriptor in LotDescription)
                    {
                        R_DESC Lot = new R_DESC(NBPackDst, LotDescriptor, false);
                        int X = Lot.Top;
                        int Y = Lot.Left;
                        // ToDo: Average of bounding box?
                        float Z = fTerrain[Y, X];
                        if (Z < TS2WaterLevel)
                            Z = TS2WaterLevel + iSpaceAboveTerrain;
                        Lot.Elevation = Z;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    String.Format("Unexpected error during copy:\n{0}", e.Message),
                    "Copy Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private R_Terrain SrcTerrain(ref GeneratableFile NBPackSrc)
        {
            // Find the source NHTG data:
            R_Terrain rSrc;
            try
            {
                if (bSC4Src)
                {
                    IPackedFileDescriptor SC4Src = NBPackSrc.FindFile(0xA9DD6FF4, 0, 0xE98F9525, 1);
                    rSrc = new R_SC4(NBPackSrc, SC4Src);
                }
                else
                {
                    IPackedFileDescriptor NHTGSrc = NBPackSrc.FindFile(0xABCB5DA4, 0, 0xFFFFFFFF, 0);
                    rSrc = new R_NHTG(NBPackSrc, NHTGSrc);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    String.Format("Cannot read terrain from source neighborhood:\n{0}\n\n{1}", sSrcNbhdName, e.Message),
                    "Copy Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return rSrc;
        }

        private R_Terrain DstTerrain(ref GeneratableFile NBPackDst)
        {
            // Find the destination NHTG:
            R_Terrain rDst;
            try
            {
                if (bSC4Dst)
                {
                    IPackedFileDescriptor SC4Dst = NBPackDst.FindFile(0xA9DD6FF4, 0, 0xE98F9525, 1);
                    rDst = new R_SC4(NBPackDst, SC4Dst);
                }
                else
                {
                    IPackedFileDescriptor NHTGDst = NBPackDst.FindFile(0xABCB5DA4, 0, 0xFFFFFFFF, 0);
                    rDst = new R_NHTG(NBPackDst, NHTGDst);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    String.Format("Cannot read terrain from destination neighborhood:\n{0}\n\n{1}", sDstNbhdName, e.Message),
                    "Copy Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return rDst;
        }

        private bool ReplaceNHTG(ref R_Terrain rSrc, ref R_Terrain rDst)
        {
            bool bIncrease = (iSrcHeight < iDstHeight) && (iSrcWidth < iDstWidth);
            bool bDecrease = (iSrcHeight > iDstHeight) && (iSrcWidth > iDstWidth);
            int iPercentElevation = 100;

            if ((iSrcHeight == iDstHeight) && (iSrcWidth == iDstWidth))
            {
                if ((bSC4Src && bSC4Dst) || (!bSC4Src && !bSC4Dst))
                {
                    // No additional manipulation required; replace the entire NHTG record
                    rDst.Raw = rSrc.Raw;
                    return true;
                }
            }
            else if (bIncrease || bDecrease)
            {
                // Mismatched neighborhood sizes, must pad or truncate
                if (ReplRoads.Checked || ReplBridges.Checked || ReplTrees.Checked || ReplDeco.Checked)
                {
                    if (DialogResult.Cancel == MessageBox.Show(
                        String.Format("Terrain sizes are different.\nCannot copy Roads, Bridges, Trees, or Decorations:\n{0}\n{1}", sSrcNbhdName, sDstNbhdName),
                        "Copy Failed!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error))
                        return false;
                    ReplRoads.Checked = ReplBridges.Checked = ReplTrees.Checked = ReplDeco.Checked = false;
                    // FixElevation.Enabled = false;
                }
                ResizeTerrain fResize = new ResizeTerrain(iSrcHeight, iSrcWidth, iDstHeight, iDstWidth);
                if (bIncrease)
                {
                    if (bSC4Src)
                        fResize.ElevationUpDown.Value = 121;
                }
                if (DialogResult.OK != fResize.ShowDialog())
                    return false;
                bResizeArrays = true;
                bScaleArrays = fResize.ResizeToFit.Checked;
                if (!bScaleArrays)
                {
                    if (bIncrease)
                    {
                        iDstTop = (int)fResize.TopUpDown.Value;
                        iDstLeft = (int)fResize.LeftUpDown.Value;
                        iDstHeight = iSrcHeight;
                        iDstWidth = iSrcWidth;
                    }
                    else if (bDecrease)
                    {
                        iSrcTop = (int)fResize.TopUpDown.Value;
                        iSrcLeft = (int)fResize.LeftUpDown.Value;
                        iSrcHeight = iDstHeight;
                        iSrcWidth = iDstWidth;
                    }
                }
                if (fResize.ScaleElevation.Checked)
                    iPercentElevation = (int)(fResize.ElevationUpDown.Value);
            }
            else
            {
                // Mismatched neighborhood sizes, cannot handle this situation
                MessageBox.Show(
                    String.Format("Terrain sizes are too different:\n{0}\n{1}", sSrcNbhdName, sDstNbhdName),
                    "Copy Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            fTerrain = rSrc.GetTerrain(iSrcTop, iSrcLeft, iSrcHeight, iSrcWidth);
            if (bSC4Src)
                ConvertSC4ToTS2(ref fTerrain, iSrcHeight, iSrcWidth);
            // Elevation is in TS2 standard values
            if (bScaleArrays)
            {
                if( bIncrease)
                    fTerrain = ResizeIncrease(fTerrain, iSrcHeight, iSrcWidth, iDstHeight, iDstWidth, iPercentElevation);
                else
                    fTerrain = ResizeDecrease(fTerrain, iSrcHeight, iSrcWidth, iDstHeight, iDstWidth, iPercentElevation);
            }
            if (bSC4Dst)
                ConvertTS2ToSC4(ref fTerrain, iDstHeight, iDstWidth);
            rDst.ReplaceTerrain(iDstTop, iDstLeft, iDstHeight, iDstWidth, fTerrain);
            return true;
        }

        // Convert elevation from SC4 to TS2 range
        private void ConvertSC4ToTS2(ref float[,] fTerrain, int iHeight, int iWidth)
        {
            // We are working with vertices, rather than tiles, so convert:
            iHeight++; iWidth++;

            for (int i = 0; i < iWidth; i++)
            {
                for (int j = 0; j < iHeight; j++)
                {
                    fTerrain[i, j] += SC4Adjustment;
                    if (Test_PrintDebugInfo)
                    {
                        if (fTerrain[i, j] > TS2WaterLevel)
                            Debug.Print("Terrain[{0},{1}] is land", i, j);
                        else
                            Debug.Print("Terrain[{0},{1}] is water", i, j);
                    }
                }
            }
        }

        // Convert elevation from TS2 to SC4 range
        private void ConvertTS2ToSC4(ref float[,] fTerrain, int iHeight, int iWidth)
        {
            // We are working with vertices, rather than tiles, so convert:
            iHeight++; iWidth++;

            for (int i = 0; i < iWidth; i++)
            {
                for (int j = 0; j < iHeight; j++)
                {
                    fTerrain[i, j] -= SC4Adjustment;
                }
            }
        }

        private float[,] ResizeDecrease(float[,] fTerrain,
            int iSrcHeight, int iSrcWidth, int iDstHeight, int iDstWidth, int iPercent)
        {
            // Determine the scale factors; source must be a multiple of destination
            int iScaleWidth = iSrcWidth / iDstWidth;
            int iScaleHeight = iSrcHeight / iDstHeight;
            if ((0 != (iSrcWidth % iDstWidth))
             || (0 != (iSrcHeight % iDstHeight)))
                throw new InvalidDataException("Invalid scale factor");

            // We are working with vertices, rather than tiles, so convert:
            iSrcHeight++; iSrcWidth++; iDstHeight++; iDstWidth++;

            // To scale down, take a sampling of the source array
            float[,] fNewTerrain = new float[iDstWidth, iDstHeight];
            for (int iFromX = 0, iToX = 0; iToX < iDstWidth; iFromX += iScaleWidth, iToX++)
            {
                for (int iFromY = 0, iToY = 0; iToY < iDstHeight; iFromY += iScaleHeight, iToY++)
                {
                    fNewTerrain[iToX, iToY] = fTerrain[iFromX, iFromY];
                    if (100 != iPercent)
                        fNewTerrain[iToX, iToY] =
                            ((fNewTerrain[iToX, iToY] - TS2WaterLevel) * (float)(iPercent)) / 100.0F
                            + TS2WaterLevel;
                }
            }
            return fNewTerrain;
        }

        private float[,] ResizeIncrease(float[,] fTerrain,
            int iSrcHeight, int iSrcWidth, int iDstHeight, int iDstWidth, int iPercent)
        {
            // Determine the scale factors; destination must be a multiple of source
            int iScaleWidth = iDstWidth / iSrcWidth;
            int iScaleHeight = iDstHeight / iSrcHeight;
            if ((0 != (iDstWidth % iSrcWidth))
             || (0 != (iDstHeight % iSrcHeight)))
                throw new InvalidDataException("Invalid scale factor");

            // We are working with vertices, rather than tiles, so convert:
            iSrcHeight++; iSrcWidth++; iDstHeight++; iDstWidth++;

            // To scale up, smooth between major vertices
            float[,] fNewTerrain = new float[iDstWidth, iDstHeight];
            for (int iFromX = 0, iToX = 0; iToX < iDstWidth; iFromX++, iToX += iScaleWidth)
            {
                for (int iFromY = 0, iToY = 0; iToY < iDstHeight; iFromY++, iToY += iScaleHeight)
                {
                    fNewTerrain[iToX, iToY] = fTerrain[iFromX, iFromY];
                    if (100 != iPercent)
                        fNewTerrain[iToX, iToY] =
                            ((fTerrain[iFromX, iFromY] - TS2WaterLevel) * (float)(iPercent)) / 100.0F
                            + TS2WaterLevel;
                }
                SmoothY(ref fNewTerrain, iToX, iDstHeight, iScaleHeight);
            }
            for (int iToY = 0; iToY < iDstHeight; iToY++)
            {
                SmoothX(ref fNewTerrain, iToY, iDstWidth, iScaleWidth);
            }
            return fNewTerrain;
        }

        // Smooth Y for a specified X
        private void SmoothY(ref float[,] fArray, int x, int iMaxY, int iScaleY)
        {
            int y = 0;
            float fPrev = fArray[x, y];
            if (Test_PrintDebugInfo)
                Debug.Print("{0},{1} Value {2}", x, y, fArray[x, y]);
            for (int j = iScaleY; j < iMaxY; j += iScaleY)
            {
                float fNext = fArray[x, j];
                float fIncrement = (fNext - fPrev) / iScaleY;
                for (int k = 1; k < iScaleY; k++)
                {
                    y = j - iScaleY + k;
                    float fNew = fPrev + k * fIncrement;
                    if (Test_PrintDebugInfo)
                        Debug.Print("{0},{1} From {2} To {3}", x, y, fArray[x, y], fNew);
                    fArray[x, y] = fNew;
                }
                if (Test_PrintDebugInfo)
                {
                    y = j;
                    Debug.Print("{0},{1} Value {2}", x, y, fArray[x, y]);
                }
                fPrev = fNext;
            }
        }

        // Smooth X for a specified Y
        private void SmoothX(ref float[,] fArray, int y, int iMaxX, int iScaleX)
        {
            int x = 0;
            float fPrev = fArray[x, y];
            if (Test_PrintDebugInfo)
                Debug.Print("{0},{1} Value {2}", x, y, fArray[x, y]);
            for (int j = iScaleX; j < iMaxX; j += iScaleX)
            {
                float fNext = fArray[j, y];
                float fIncrement = (fNext - fPrev) / iScaleX;
                for (int k = 1; k < iScaleX; k++)
                {
                    x = j - iScaleX + k;
                    float fNew = fPrev + k * fIncrement;
                    if (Test_PrintDebugInfo)
                        Debug.Print("{0},{1} From {2} To {3}", x, y, fArray[x, y], fNew);
                    fArray[x, y] = fNew;
                }
                if (Test_PrintDebugInfo)
                {
                    x = j;
                    Debug.Print("{0},{1} Value {2}", x, y, fArray[x, y]);
                }
                fPrev = fNext;
            }
        }

        private bool ReplaceNHTR(ref GeneratableFile NBPackSrc, ref GeneratableFile NBPackDst)
        {
            R_NHTR rSrc = null;
            try
            {
                // No need for Source NHTR unless we are actually replacing something.
                if (ReplRoads.Checked || ReplBridges.Checked || ReplTrees.Checked || ReplDeco.Checked)
                {
                    IPackedFileDescriptor NHTRSrc = NBPackSrc.FindFile(0xABD0DC63, 0, 0xFFFFFFFF, 0);
                    rSrc = new R_NHTR(NBPackSrc, NHTRSrc);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    String.Format("Cannot parse structures in source neighborhood:\n{0}\n\n{1}", sSrcNbhdName, e.Message),
                    "Copy Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            R_NHTR rDst;
            try
            {
                IPackedFileDescriptor NHTRDst = NBPackDst.FindFile(0xABD0DC63, 0, 0xFFFFFFFF, 0);
                rDst = new R_NHTR(NBPackDst, NHTRDst);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    String.Format("Cannot parse structures in destination neighborhood:\n{0}\n\n{1}", sDstNbhdName, e.Message),
                    "Copy Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                if (ReplTrees.Checked)
                    rDst.Trees = rSrc.Trees;
                if (ReplRoads.Checked)
                    rDst.Roads = rSrc.Roads;
                if (ReplBridges.Checked)
                    rDst.Bridges = rSrc.Bridges;
                if (ReplDeco.Checked)
                    rDst.Deco = rSrc.Deco;

                if (FixTrees.Checked)
                    rDst.FixTreeElevations(fTerrain);
                if (FixRoads.Checked)
                    rDst.FixRoadElevations(fTerrain);
                if (FixBridges.Checked)
                    rDst.FixBridgeElevations(fTerrain);
                if (FixDeco.Checked)
                    rDst.FixDecoElevations(fTerrain);

                if (DelTrees.Checked)
                    rDst.DeleteAllTrees();
                if (DelRoads.Checked)
                    rDst.DeleteAllRoads();
                if (DelBridges.Checked)
                    rDst.DeleteAllBridges();
                if (DelDeco.Checked)
                    rDst.DeleteAllDeco();

                // if (bResizeArrays)
                //     rDst.ResizeArrays(iTop, iLeft, iHeight, iWidth);
                // throw new InvalidDataException("Test Exception Handling");

                rDst.Rewrite();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    String.Format("Cannot copy structures from {0} to {1}\n\n{2}", sSrcTrimName, sDstTrimName, e.Message),
                    "Copy Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private uint MaxBKP(string sPath, string sExt, uint uMaxBKP)
        {
            string sDir = Path.GetDirectoryName(sPath);
            string sName = Path.GetFileNameWithoutExtension(sPath);

            string sBKP = string.Concat(sName, "_*");
            sBKP = Path.ChangeExtension(sBKP, sExt);

            // Find unique package file number for backup
            string[] sNames = System.IO.Directory.GetFiles(sDir, sBKP);
            for (int i = 0; i < sNames.Length; i++)
            {
                string s1 = Path.GetFileNameWithoutExtension(sNames[i]);
                string s2 = s1.Substring(sName.Length + 1);
                uint uCurrent = 0;
                try
                {
                    uCurrent = uint.Parse(s2);
                }
                catch (FormatException)
                {
                    continue;
                }
                if (uMaxBKP <= uCurrent)
                    uMaxBKP = uCurrent + 1;
            }
            return uMaxBKP;
        }

        private string BackupFileName(GeneratableFile Package, string sExt, bool bVersion, uint uVersion)
        {
            string sPath = Package.FileName;
            if (bVersion)
            {
                string sDir = Path.GetDirectoryName(sPath);
                string sName = Path.GetFileNameWithoutExtension(sPath);

                sPath = string.Concat(sDir, Path.DirectorySeparatorChar, sName);
                sPath = string.Concat(sPath, "_", uVersion.ToString());
                sPath = Path.ChangeExtension(sPath, sExt);  // add in backup extension
            }
            else
                sPath = Path.ChangeExtension(sPath, sExt);
            return sPath;
        }

        private void PackageSave(GeneratableFile Package)
        {
            string sExt = ".bkp";
            uint uMaxBKP = 0;
            if (bBackupVersioning)
                uMaxBKP = MaxBKP(Package.FileName, sExt, uMaxBKP);

            // This method exists only because SimPE GeneratableFile.Save() does not work here!
            string sPath = BackupFileName(Package, sExt, bBackupVersioning, uMaxBKP);
            System.IO.File.Copy(Package.FileName, sPath, true);
            MemoryStream MS = Package.Build();
            Package.Close();
            FileStream FS = new FileStream(Package.FileName, FileMode.Create, FileAccess.ReadWrite);
            FS.Seek(0, SeekOrigin.Begin);
            FS.SetLength(0);
            byte[] B = MS.ToArray();
            FS.Write(B, 0, B.Length);
            FS.Close();
        }

        private void ReplRoads_CheckedChanged(object sender, EventArgs e)
        {
            if (ReplRoads.Checked)
                DelRoads.Checked = false;
        }

        private void ReplBridges_CheckedChanged(object sender, EventArgs e)
        {
            if (ReplBridges.Checked)
                DelBridges.Checked = false;
        }

        private void ReplTrees_CheckedChanged(object sender, EventArgs e)
        {
            if (ReplTrees.Checked)
                DelTrees.Checked = false;
            else if (!DelTrees.Checked)
                FixTrees.Checked = true;
        }

        private void ReplDeco_CheckedChanged(object sender, EventArgs e)
        {
            if (ReplDeco.Checked)
                DelDeco.Checked = false;
            else if (!DelDeco.Checked)
                FixDeco.Checked = true;
        }

        private void DelRoads_CheckedChanged(object sender, EventArgs e)
        {
            if (DelRoads.Checked)
                ReplRoads.Checked = FixRoads.Checked = false;
        }

        private void DelBridges_CheckedChanged(object sender, EventArgs e)
        {
            if (DelBridges.Checked)
                ReplBridges.Checked = FixBridges.Checked = false;
        }

        private void DelTrees_CheckedChanged(object sender, EventArgs e)
        {
            if (DelTrees.Checked)
                ReplTrees.Checked = FixTrees.Checked = false;
        }

        private void DelDeco_CheckedChanged(object sender, EventArgs e)
        {
            if (DelDeco.Checked)
                ReplDeco.Checked = FixDeco.Checked = false;
        }

        private void MultiBackup_CheckedChanged(object sender, EventArgs e)
        {
            bBackupVersioning = MultiBackup.Checked;
        }

/*      private void SC4Save(GeneratableFile Package)
        {
            // This method exists because SimPE can't save compressed SC4 files
            // ToDo: Get this working...
            //       The problem is that the SimPE DLLs write out a different version than they read in.
            System.IO.File.Copy(Package.FileName, Path.ChangeExtension(Package.FileName, ".bkp"), true);
            FileStream FS = new FileStream(Package.FileName, FileMode.Create, FileAccess.ReadWrite);
            FS.Seek(0, SeekOrigin.Begin);
            FS.SetLength(0);
            // ToDo: Write header
            IPackedFileDescriptor[] Descriptor;
            Descriptor = Package.Index;
            int i = 0;
            foreach (IPackedFileDescriptor IPFD in Descriptor)
            {
                IPackedFile PF = Package.Read(IPFD);
                byte[] Data = PF.UncompressedData;
                FS.Write(Data, 0, Data.Length);
            }
        }
 */
    }
}