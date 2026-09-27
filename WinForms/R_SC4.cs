/***************************************************************************
 *   Copyright (C) 2008 by Mootilda                                        *
 *   http://Mootilda.ModTheSims2.com                                       *
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SimPe.Interfaces.Files;
using SimPe.Packages;


/* Unfortunately, the SimPE DLLs do not seem to be able to write out an SC4 file correctly.
 * SimCity 4 no longer recognizes changed files as city files.
 * Possible solutions:
 * - Turn off compression. Probably not the problem.
 * - Write out header, index table, etc, myself.  Verified that SC4 has version 0; SimPE DLLs write out version 1.
 */
namespace HoodReplace
{
    public class R_SC4 : R_Terrain
    {
        private IPackedFileDescriptor PFD;
        private byte[] Data;
        // private byte[] DataOrig;
        private const int iSmallGrid = 64;
        private const int iMediumGrid = 128;
        private const int iLargeGrid = 256;
        private int iGridWidth;
        private int iGridHeight;
        private int iHeaderSize = 2;

        public R_SC4(IPackageFile NBPack, IPackedFileDescriptor Descriptor)
        {
            PFD = Descriptor;
            IPackedFile PF = NBPack.Read(PFD);
            Data = PF.UncompressedData;
            // DataOrig = new byte[Data.Length];
            // Array.Copy(Data, DataOrig, Data.Length);

            BinaryReader BR = SimPe.Helper.GetBinaryReader(Data);
            uint uBlockVersion = BR.ReadUInt16();
            Debug.Assert(uBlockVersion == 2);   // ToDo: Determine whether other versions are known and handled correctly
            int iIndex = 2;

            if (Data.Length == ((iSmallGrid + 1) * (iSmallGrid + 1) * 4 + iIndex))
            {
                iGridHeight = iSmallGrid;
                iGridWidth = iSmallGrid;
            }
            else if (Data.Length == ((iMediumGrid + 1) * (iMediumGrid + 1) * 4 + iIndex))
            {
                iGridHeight = iMediumGrid;
                iGridWidth = iMediumGrid;
            }
            else if (Data.Length == ((iLargeGrid + 1) * (iLargeGrid + 1) * 4 + iIndex))
            {
                iGridHeight = iLargeGrid;
                iGridWidth = iLargeGrid;
            }
            else
            {
                Debug.Fail("Unknown SimCity 4 city size");
            }

            iHeaderSize = iIndex;   // Size varies, depending upon terrain type
        }

        public override int Width
        {
            get
            {
                return this.iGridWidth;
            }
        }

        public override int Height
        {
            get
            {
                return this.iGridHeight;
            }
        }

        public override byte[] Raw
        {
            get
            {
                return this.Data;
            }
            set
            {
                PFD.SetUserData(value, true);
            }
        }

        public override float[,] GetTerrain(int iTop, int iLeft, int iHeight, int iWidth)
        {
            if ((iTop + iHeight) > iGridHeight)
                throw new IndexOutOfRangeException("Invalid Terrain Height");
            if ((iLeft + iWidth) > iGridWidth)
                throw new IndexOutOfRangeException("Invalid Terrain Width");

            // We are working with vertices, rather than tiles, so convert:
            iHeight++;
            iWidth++;

            BinaryReader BR = SimPe.Helper.GetBinaryReader(Data);
            BR.ReadBytes(iHeaderSize);

            float[,] fTerrainData = new float[iGridWidth + 1, iGridHeight + 1];
            for (int i = 0; i < (iGridWidth + 1); i++)
            {
                for (int j = 0; j < (iGridHeight + 1); j++)
                {
                    fTerrainData[i,j] = BR.ReadSingle();
                }
            }

            float[,] fTerrain = new float[iWidth, iHeight];
            for (int iFromX = iLeft, iToX = 0; iToX < iWidth; iFromX++, iToX++)
            {
                for (int iFromY = iTop, iToY = 0; iToY < iHeight; iFromY++, iToY++)
                {
                    fTerrain[iToX, iToY] = fTerrainData[iFromX, iFromY];
                }
            }
            return fTerrain;
        }

        public override void ReplaceTerrain(int iTop, int iLeft, int iHeight, int iWidth, float[,] fTerrain)
        {
            if ((iTop + iHeight) > iGridHeight)
                throw new IndexOutOfRangeException("Invalid Terrain Height");
            if ((iLeft + iWidth) > iGridWidth)
                throw new IndexOutOfRangeException("Invalid Terrain Width");

            // We are working with vertices, rather than tiles, so convert:
            iHeight++;
            iWidth++;

            byte[] bDataNew = new byte[Data.Length];
            Array.Copy(Data, bDataNew, Data.Length);

            BinaryReader BR = SimPe.Helper.GetBinaryReader(Data);
            BinaryWriter BW = new BinaryWriter(new MemoryStream(bDataNew));
            BW.Write(BR.ReadBytes(iHeaderSize));

            float[,] fTerrainData = new float[iGridWidth + 1, iGridHeight + 1];
            for (int i = 0; i < (iGridWidth + 1); i++)
            {
                for (int j = 0; j < (iGridHeight + 1); j++)
                {
                    fTerrainData[i, j] = BR.ReadSingle();
                }
            }

            for (int iFromX = 0, iToX = iLeft; iFromX < iWidth; iFromX++, iToX++)
            {
                for (int iFromY = 0, iToY = iTop; iFromY < iHeight; iFromY++, iToY++)
                {
                    fTerrainData[iToX, iToY] = fTerrain[iFromX, iFromY];
                }
            }

            for (int i = 0; i < (iGridWidth + 1); i++)
            {
                for (int j = 0; j < (iGridHeight + 1); j++)
                {
                    BW.Write(fTerrainData[i, j]);
                }
            }
            PFD.SetUserData(bDataNew, true);
        }
    }
}