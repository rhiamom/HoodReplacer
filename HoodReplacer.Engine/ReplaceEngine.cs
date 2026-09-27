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
 ***************************************************************************/

// Mootilda's replace logic, lifted out of HoodReplace.cs (the WinForms form)
// so it can run headless. Her code is preserved verbatim apart from the UI
// touchpoints, which had to change because there is no form here:
//
//   * 16 `SomeCheckBox.Checked` reads  -> `_o.<Option>` on ReplaceOptions
//   * 7  error MessageBox.Show(...)    -> Fail(...), recorded on the report
//   * 1  OKCancel MessageBox.Show(...) -> Confirm(...), a caller-supplied
//        callback. This one is a real decision, not an error: when the two
//        terrains differ in size, continuing means the Roads/Bridges/Trees/
//        Decorations options get dropped. Headless callers must answer it.
//   * ReplaceFile() read the two ListBox selections, so it is replaced by
//     Run(srcPath, dstPath, options) below. The rest of its behaviour --
//     open both, replace, ForgetUpdate the source, PackageSave the
//     destination -- is unchanged.
//
// Nothing else was reshaped. See CLAUDE.md on why her code is treated as
// archival.

#nullable disable

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SimPe.Interfaces.Files;
using SimPe.Packages;

namespace HoodReplace
{
    /// <summary>Replaces the form's check boxes.</summary>
    public sealed class ReplaceOptions
    {
        public bool ReplaceTerrain { get; set; }
        public bool ReplaceRoads { get; set; }
        public bool ReplaceBridges { get; set; }
        public bool ReplaceTrees { get; set; }
        public bool ReplaceDecorations { get; set; }

        public bool FixRoads { get; set; }
        public bool FixBridges { get; set; }
        public bool FixTrees { get; set; }
        public bool FixDecorations { get; set; }
        public bool FixLots { get; set; }

        public bool DeleteRoads { get; set; }
        public bool DeleteBridges { get; set; }
        public bool DeleteTrees { get; set; }
        public bool DeleteDecorations { get; set; }

        public bool ResizeToFit { get; set; }
        public bool ScaleElevation { get; set; }

        /// <summary>Keep numbered .bkp files instead of overwriting one.</summary>
        public bool VersionedBackups { get; set; }
    }

    /// <summary>What the ResizeTerrain dialog used to ask about.</summary>
    public sealed class ResizeRequest
    {
        public int SrcHeight, SrcWidth, DstHeight, DstWidth;
        public bool Increase;
        public int PercentElevation;
    }

    /// <summary>The answer. Return null to cancel (old DialogResult != OK).</summary>
    public sealed class ResizeChoice
    {
        /// <summary>Scale the terrain to fit rather than placing it at Top/Left.</summary>
        public bool ResizeToFit { get; set; }
        public bool ScaleElevation { get; set; }
        public int PercentElevation { get; set; } = 100;
        public int Top { get; set; }
        public int Left { get; set; }
    }

    public sealed class ReplaceReport
    {
        public bool Changed { get; internal set; }
        public bool Saved { get; internal set; }
        public List<string> Errors { get; } = new List<string>();
        public List<string> Log { get; } = new List<string>();
        public string BackupPath { get; internal set; }
    }

    public sealed class ReplaceEngine
    {
        private ReplaceOptions _o = new ReplaceOptions();
        private ReplaceReport _r = new ReplaceReport();

        /// <summary>
        /// Answers the "terrain sizes differ, continue without structures?"
        /// prompt. Return true to continue. Defaults to false (abort), so a
        /// headless caller never silently discards options.
        /// </summary>
        public Func<string, bool> ConfirmHandler { get; set; }

        private bool Confirm(string message)
        {
            _r.Log.Add("CONFIRM: " + message);
            return ConfirmHandler != null && ConfirmHandler(message);
        }

        /// <summary>
        /// Answers the resize question when the two terrains differ in size.
        /// Null (the default) cancels, so a headless caller never silently
        /// invents placement or elevation scaling.
        /// </summary>
        public Func<ResizeRequest, ResizeChoice> ResizeHandler { get; set; }

        private ResizeChoice RequestResize(ResizeRequest request)
        {
            _r.Log.Add(string.Format(
                "RESIZE: source {0}x{1} -> destination {2}x{3} ({4})",
                request.SrcWidth, request.SrcHeight, request.DstWidth, request.DstHeight,
                request.Increase ? "increase" : "decrease"));
            return ResizeHandler != null ? ResizeHandler(request) : null;
        }

        private void Fail(string message)
        {
            _r.Errors.Add(message);
        }

        // ---- fields carried over from the form ---------------------------
        private bool Test_PrintDebugInfo = false;
        private bool bBackupVersioning = false;

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

        /// <summary>
        /// Headless replacement for ReplaceFile(): copies terrain (and, if
        /// asked, structures) from <paramref name="srcPath"/> into
        /// <paramref name="dstPath"/>. Lots are never copied -- FixLots only
        /// re-seats the destination's own lots onto the new terrain.
        /// The destination is backed up to .bkp before it is rewritten.
        /// </summary>
        public ReplaceReport Run(string srcPath, string dstPath, ReplaceOptions options)
        {
            _o = options ?? new ReplaceOptions();
            _r = new ReplaceReport();
            bBackupVersioning = _o.VersionedBackups;

            bSC4Src = string.Equals(Path.GetExtension(srcPath), sSC4Ext, StringComparison.OrdinalIgnoreCase);
            bSC4Dst = string.Equals(Path.GetExtension(dstPath), sSC4Ext, StringComparison.OrdinalIgnoreCase);

            sSrcNbhdName = Path.GetFileNameWithoutExtension(srcPath);
            sDstNbhdName = Path.GetFileNameWithoutExtension(dstPath);
            sSrcTrimName = sSrcNbhdName;
            sDstTrimName = sDstNbhdName;

            GeneratableFile NBPackSrc;
            try
            {
                NBPackSrc = SimPe.Packages.File.LoadFromFile(srcPath);
            }
            catch (Exception e)
            {
                Fail(string.Format("Cannot open source neighborhood:\n{0}\n\n{1}", sSrcNbhdName, e.Message));
                return _r;
            }

            GeneratableFile NBPackDst;
            try
            {
                NBPackDst = SimPe.Packages.File.LoadFromFile(dstPath);
            }
            catch (Exception e)
            {
                Fail(string.Format("Cannot open destination neighborhood:\n{0}\n\n{1}", sDstNbhdName, e.Message));
                NBPackSrc.ForgetUpdate();
                NBPackSrc.Close();
                return _r;
            }

            bool bChanged = ReplaceRecords(ref NBPackSrc, ref NBPackDst);

            // Abort any changes to the source neighborhood (as the original did).
            NBPackSrc.ForgetUpdate();
            NBPackSrc.Close();

            if (bChanged)
            {
                _r.BackupPath = BackupFileName(NBPackDst, ".bkp", bBackupVersioning,
                                   bBackupVersioning ? MaxBKP(NBPackDst.FileName, ".bkp", 0) : 0);
                PackageSave(NBPackDst);
                _r.Saved = true;
                _r.Log.Add(string.Format("Copied from {0} to {1}.", sSrcTrimName, sDstTrimName));
            }
            else
            {
                NBPackDst.ForgetUpdate();
                NBPackDst.Close();
            }

            _r.Changed = bChanged;
            return _r;
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
                if (_o.ReplaceTerrain)
                {
                    if (!ReplaceNHTG(ref rSrc, ref rDst))
                        return false;
                }
                if (_o.ReplaceRoads || _o.ReplaceBridges || _o.ReplaceTrees || _o.ReplaceDecorations
                 || _o.FixRoads || _o.FixBridges || _o.FixTrees || _o.FixDecorations || _o.FixLots
                 || _o.DeleteRoads || _o.DeleteBridges || _o.DeleteTrees || _o.DeleteDecorations)
                {
                    if ((fTerrain == null)
                     && (_o.FixRoads || _o.FixBridges || _o.FixTrees || _o.FixDecorations || _o.FixLots))
                    {
                        if (_o.ReplaceTerrain)
                        {
                            // If (_o.ReplaceTerrain) && (fTerrain == null),
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
                if (_o.FixLots)
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
                Fail(String.Format("Unexpected error during copy:\n{0}", e.Message));
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
                Fail(String.Format("Cannot read terrain from source neighborhood:\n{0}\n\n{1}", sSrcNbhdName, e.Message));
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
                Fail(String.Format("Cannot read terrain from destination neighborhood:\n{0}\n\n{1}", sDstNbhdName, e.Message));
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
                if (_o.ReplaceRoads || _o.ReplaceBridges || _o.ReplaceTrees || _o.ReplaceDecorations)
                {
                    if (!Confirm(String.Format("Terrain sizes are different.\nCannot copy Roads, Bridges, Trees, or Decorations:\n{0}\n{1}", sSrcNbhdName, sDstNbhdName)))
                        return false;
                    _o.ReplaceRoads = _o.ReplaceBridges = _o.ReplaceTrees = _o.ReplaceDecorations = false;
                    // FixElevation.Enabled = false;
                }
                // The original opened the ResizeTerrain form here to collect
                // placement/scaling. Headless, that becomes a request the caller
                // answers; returning null is the old DialogResult != OK path.
                ResizeRequest request = new ResizeRequest
                {
                    SrcHeight = iSrcHeight,
                    SrcWidth = iSrcWidth,
                    DstHeight = iDstHeight,
                    DstWidth = iDstWidth,
                    Increase = bIncrease,
                    // Her default: growing from an SC4 source pre-seeds 121%.
                    PercentElevation = (bIncrease && bSC4Src) ? 121 : 100,
                };
                ResizeChoice choice = RequestResize(request);
                if (choice == null)
                    return false;
                bResizeArrays = true;
                bScaleArrays = choice.ResizeToFit;
                if (!bScaleArrays)
                {
                    if (bIncrease)
                    {
                        iDstTop = choice.Top;
                        iDstLeft = choice.Left;
                        iDstHeight = iSrcHeight;
                        iDstWidth = iSrcWidth;
                    }
                    else if (bDecrease)
                    {
                        iSrcTop = choice.Top;
                        iSrcLeft = choice.Left;
                        iSrcHeight = iDstHeight;
                        iSrcWidth = iDstWidth;
                    }
                }
                if (choice.ScaleElevation)
                    iPercentElevation = choice.PercentElevation;
            }
            else
            {
                // Mismatched neighborhood sizes, cannot handle this situation
                Fail(String.Format("Terrain sizes are too different:\n{0}\n{1}", sSrcNbhdName, sDstNbhdName));
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
                if (_o.ReplaceRoads || _o.ReplaceBridges || _o.ReplaceTrees || _o.ReplaceDecorations)
                {
                    IPackedFileDescriptor NHTRSrc = NBPackSrc.FindFile(0xABD0DC63, 0, 0xFFFFFFFF, 0);
                    rSrc = new R_NHTR(NBPackSrc, NHTRSrc);
                }
            }
            catch (Exception e)
            {
                Fail(String.Format("Cannot parse structures in source neighborhood:\n{0}\n\n{1}", sSrcNbhdName, e.Message));
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
                Fail(String.Format("Cannot parse structures in destination neighborhood:\n{0}\n\n{1}", sDstNbhdName, e.Message));
                return false;
            }

            try
            {
                if (_o.ReplaceTrees)
                    rDst.Trees = rSrc.Trees;
                if (_o.ReplaceRoads)
                    rDst.Roads = rSrc.Roads;
                if (_o.ReplaceBridges)
                    rDst.Bridges = rSrc.Bridges;
                if (_o.ReplaceDecorations)
                    rDst.Deco = rSrc.Deco;

                if (_o.FixTrees)
                    rDst.FixTreeElevations(fTerrain);
                if (_o.FixRoads)
                    rDst.FixRoadElevations(fTerrain);
                if (_o.FixBridges)
                    rDst.FixBridgeElevations(fTerrain);
                if (_o.FixDecorations)
                    rDst.FixDecoElevations(fTerrain);

                if (_o.DeleteTrees)
                    rDst.DeleteAllTrees();
                if (_o.DeleteRoads)
                    rDst.DeleteAllRoads();
                if (_o.DeleteBridges)
                    rDst.DeleteAllBridges();
                if (_o.DeleteDecorations)
                    rDst.DeleteAllDeco();

                // if (bResizeArrays)
                //     rDst.ResizeArrays(iTop, iLeft, iHeight, iWidth);
                // throw new InvalidDataException("Test Exception Handling");

                rDst.Rewrite();
            }
            catch (Exception e)
            {
                Fail(String.Format("Cannot copy structures from {0} to {1}\n\n{2}", sSrcTrimName, sDstTrimName, e.Message));
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

    }
}
