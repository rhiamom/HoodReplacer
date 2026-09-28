/**************************************************************************
 *   HoodReplacer for Mac                                                 *
 *   HoodReplace © 2008-2010 Mootilda (http://Mootilda.ModTheSims.info)   *
 *   macOS port © 2026 GramzeSweatshop (rhiamom@mac.com)                  *
 *   Ported with Claude (Anthropic)                                       *
 *   GPL v2 or later. See Licences/GPL-LICENSE.txt                        *
 *                                                                        *
 *   Not in Mootilda's original. After a copy, the hood's preview         *
 *   picture (<code>_Neighborhood.png, shown in the neighborhood          *
 *   chooser) still showed the old terrain, and the game never redraws    *
 *   it. This swaps in the picture that belongs to the new terrain:       *
 *     - source is a neighborhood: its own <code>_Neighborhood.png        *
 *     - source is an .sc4: a .png shipped beside it. The game's          *
 *       SC4Terrains folder names these <name>.png plus <name>_lush,      *
 *       _desert, _dirt, _concrete; the variant matching the              *
 *       destination's terrain type wins. Downloads are less tidy ("City  *
 *       - Diamond Lake.sc4" beside "Diamond%20Lake.png", or a lone       *
 *       N011_Neighborhood.png), so names are compared loosely and a      *
 *       single .png in the folder is taken as the one.                   *
 *   The old picture is kept as <code>_Neighborhood.png.bkp.              *
 *************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace HoodReplace
{
    public static class PreviewPicture
    {
        private const uint NHTG = 0xABCB5DA4;
        private const string MainSuffix = "_Neighborhood";

        /// <summary>
        /// Replaces the destination hood's preview picture with the one that
        /// goes with <paramref name="srcPath"/>. Returns a line for the log.
        /// Never throws for "nothing to do"; the copy itself already succeeded.
        /// </summary>
        public static string Update(string srcPath, string dstPath)
        {
            string dstStem = Path.GetFileNameWithoutExtension(dstPath);
            if (!dstStem.EndsWith(MainSuffix, StringComparison.OrdinalIgnoreCase))
                return "Preview picture: only the main neighborhood has one; left alone.";

            string dstPng = Path.ChangeExtension(dstPath, ".png");
            string? srcPng = FindSourcePicture(srcPath, TerrainType(dstPath));
            if (srcPng == null)
                return "Preview picture: none found beside " + Path.GetFileName(srcPath) +
                       "; the old picture was kept.";

            if (File.Exists(dstPng))
                File.Copy(dstPng, dstPng + ".bkp", true);
            File.Copy(srcPng, dstPng, true);
            return "Preview picture: replaced with " + Path.GetFileName(srcPng) + ".";
        }

        /// <summary>The .png to use, or null if there isn't a sensible one.</summary>
        public static string? FindSourcePicture(string srcPath, string? terrainType)
        {
            string? dir = Path.GetDirectoryName(srcPath);
            if (dir == null || !Directory.Exists(dir)) return null;

            if (!string.Equals(Path.GetExtension(srcPath), ".sc4", StringComparison.OrdinalIgnoreCase))
            {
                // A neighborhood: only a main hood has a picture.
                string own = Path.ChangeExtension(srcPath, ".png");
                return Path.GetFileNameWithoutExtension(srcPath)
                           .EndsWith(MainSuffix, StringComparison.OrdinalIgnoreCase)
                       && File.Exists(own) ? own : null;
            }

            string want = Loose(Path.GetFileNameWithoutExtension(srcPath));
            string? variant = VariantFor(terrainType);
            string[] pngs = Directory.GetFiles(dir, "*.png");

            string? plain = null, sameType = null;
            foreach (string png in pngs)
            {
                string stem = Uri.UnescapeDataString(Path.GetFileNameWithoutExtension(png));
                string? suffix = null;
                int us = stem.LastIndexOf('_');
                if (us > 0 && VariantNames.Contains(stem[(us + 1)..].ToLowerInvariant()))
                {
                    suffix = stem[(us + 1)..].ToLowerInvariant();
                    stem = stem[..us];
                }
                if (Loose(stem) != want) continue;
                if (suffix == null) plain = png;
                else if (suffix == variant) sameType = png;
            }

            return sameType ?? plain ?? (pngs.Length == 1 ? pngs[0] : null);
        }

        private static readonly string[] VariantNames = { "lush", "desert", "dirt", "concrete" };

        private static string? VariantFor(string? terrainType) => terrainType switch
        {
            "Temperate" => "lush",
            "Desert" => "desert",
            "Dirt" => "dirt",
            "Concrete" => "concrete",
            _ => null,
        };

        /// <summary>"City - Diamond Lake" and "Diamond%20Lake" both become "diamondlake".</summary>
        private static string Loose(string name)
        {
            name = Uri.UnescapeDataString(name);
            if (name.StartsWith("City - ", StringComparison.OrdinalIgnoreCase))
                name = name[7..];
            return new string(name.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
        }

        /// <summary>
        /// The hood's terrain type ("Temperate", "Desert", "Dirt", "Concrete"),
        /// read from the NHTG header the same way R_NHTG does: block id,
        /// version, width, height, water level, then a length-prefixed string.
        /// </summary>
        public static string? TerrainType(string hoodPath)
        {
            try
            {
                var pkg = SimPe.Packages.File.LoadFromFile(hoodPath);
                try
                {
                    var pfd = pkg.FindFiles(NHTG).FirstOrDefault();
                    if (pfd == null) return null;
                    byte[] data = pkg.Read(pfd).UncompressedData;
                    if (data.Length < 28 || BitConverter.ToUInt32(data, 0) != NHTG) return null;
                    int len = BitConverter.ToInt32(data, 20);
                    if (len <= 0 || 24 + len > data.Length) return null;
                    return Encoding.ASCII.GetString(data, 24, len);
                }
                finally
                {
                    pkg.ForgetUpdate();
                    pkg.Close();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
