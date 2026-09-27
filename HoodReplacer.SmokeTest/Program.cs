// HoodReplacer smoke test — headless harness.
//
// Stage 1: path discovery + neighborhood catalog.
// Stage 2: terrain dimensions per hood (which pairs avoid the resize path).
// Stage 3: round-trip — copy terrain into a COPY of a hood, re-read it from
//          disk, and compare the decoded float arrays element by element.
//
// Nothing here writes to a real neighborhood. Stage 3 works on a scratch copy.

using HoodReplace;
using LotExpander;
using SimPe.Interfaces.Files;
using SimPe.Packages;

namespace HoodReplacer.SmokeTest;

internal static class Program
{
    private const uint NHTG = 0xABCB5DA4;

    private static R_Terrain? ReadTerrain(GeneratableFile pkg)
    {
        IPackedFileDescriptor d = pkg.FindFile(NHTG, 0, 0xFFFFFFFF, 0);
        return d is null ? null : new R_NHTG(pkg, d);
    }

    private static int Main(string[] args)
    {
        Console.WriteLine("HoodReplacer smoke test");
        Console.WriteLine(new string('-', 64));

        string? hoods = SimsPaths.NeighborhoodsFolder;
        if (hoods is null) { Console.WriteLine("FAIL: no Sims 2 user folder found."); return 1; }
        Console.WriteLine($"Neighborhoods: {hoods}\n");

        var list = NeighborhoodCatalog.List(hoods);
        var dims = new Dictionary<string, (int W, int H, string Path)>();

        Console.WriteLine("Terrain dimensions:");
        foreach (var n in list)
        {
            try
            {
                var pkg = SimPe.Packages.File.LoadFromFile(n.MainPackagePath);
                var t = ReadTerrain(pkg);
                if (t is null) Console.WriteLine($"   {n.Code,-6} {n.Name,-22} (no NHTG)");
                else
                {
                    dims[n.Code] = (t.Width, t.Height, n.MainPackagePath);
                    Console.WriteLine($"   {n.Code,-6} {n.Name,-22} {t.Width} x {t.Height}");
                }
                pkg.ForgetUpdate(); pkg.Close();
            }
            catch (Exception e) { Console.WriteLine($"   {n.Code,-6} {n.Name,-22} ERROR {e.Message}"); }
        }

        if (args.Length < 2)
        {
            Console.WriteLine("\nPass <srcCode> <dstCode> to run the round-trip test (dst is copied first).");
            return 0;
        }

        string srcCode = args[0], dstCode = args[1];
        if (!dims.ContainsKey(dstCode))
        { Console.WriteLine($"\nFAIL: unknown destination hood code."); return 1; }

        // Source may be a hood code or a path to an .SC4 terrain.
        int sw, sh; string srcPath;
        if (dims.ContainsKey(srcCode))
            (sw, sh, srcPath) = dims[srcCode];
        else if (System.IO.File.Exists(srcCode))
        {
            srcPath = srcCode;
            var sc4pkg = SimPe.Packages.File.LoadFromFile(srcPath);
            var d4 = sc4pkg.FindFile(0xA9DD6FF4, 0, 0xE98F9525, 1);
            if (d4 is null) { Console.WriteLine("FAIL: no SC4 terrain resource (0xA9DD6FF4) in that file."); return 1; }
            var t4 = new R_SC4(sc4pkg, d4);
            sw = t4.Width; sh = t4.Height;
            Console.WriteLine($"SC4 source: {Path.GetFileName(srcPath)}  {sw} x {sh}");
            sc4pkg.ForgetUpdate(); sc4pkg.Close();
        }
        else { Console.WriteLine($"\nFAIL: source is neither a hood code nor a file: {srcCode}"); return 1; }
        var (dw, dh, dstPath) = dims[dstCode];
        Console.WriteLine($"\nRound-trip: {srcCode} ({sw}x{sh})  ->  {dstCode} ({dw}x{dh})");

        // Work on a scratch copy so the real hood is never touched.
        string scratch = Path.Combine(Path.GetTempPath(), "hoodreplacer-smoke");
        Directory.CreateDirectory(scratch);
        string workPath = Path.Combine(scratch, Path.GetFileName(dstPath));
        System.IO.File.Copy(dstPath, workPath, true);
        Console.WriteLine($"Working copy: {workPath}");

        // BEFORE
        var dstPkg = SimPe.Packages.File.LoadFromFile(workPath);
        var before = ReadTerrain(dstPkg)!.GetTerrain(0, 0, dh, dw);
        dstPkg.ForgetUpdate(); dstPkg.Close();

        // Expected = source terrain
        bool srcIsSc4 = string.Equals(Path.GetExtension(srcPath), ".SC4", StringComparison.OrdinalIgnoreCase);
        var srcPkg = SimPe.Packages.File.LoadFromFile(srcPath);
        R_Terrain srcTerr = srcIsSc4
            ? new R_SC4(srcPkg, srcPkg.FindFile(0xA9DD6FF4, 0, 0xE98F9525, 1))
            : ReadTerrain(srcPkg)!;
        var expected = srcTerr.GetTerrain(0, 0, sh, sw);
        srcPkg.ForgetUpdate(); srcPkg.Close();

        var engine = new ReplaceEngine();
        engine.ResizeHandler = req =>
        {
            Console.WriteLine($"   resize requested: {req.SrcWidth}x{req.SrcHeight} -> {req.DstWidth}x{req.DstHeight}, scale-to-fit");
            return new ResizeChoice { ResizeToFit = true, ScaleElevation = false, PercentElevation = req.PercentElevation };
        };
        engine.ConfirmHandler = _ => true;
        var report = engine.Run(srcPath, workPath, new ReplaceOptions { ReplaceTerrain = true });

        foreach (var l in report.Log) Console.WriteLine($"   log: {l}");
        foreach (var e in report.Errors) Console.WriteLine($"   ERR: {e}");
        Console.WriteLine($"   changed={report.Changed} saved={report.Saved} backup={report.BackupPath ?? "(none)"}");
        if (!report.Saved) { Console.WriteLine("\nFAIL: nothing was saved."); return 1; }

        // AFTER — re-read from disk, not from memory
        var reread = SimPe.Packages.File.LoadFromFile(workPath);
        var after = ReadTerrain(reread)!.GetTerrain(0, 0, dh, dw);
        reread.ForgetUpdate(); reread.Close();

        bool sameSize = (sw == dw && sh == dh);
        int mismatch = 0, changed = 0;
        float worst = 0;
        for (int y = 0; y < dh; y++)
            for (int x = 0; x < dw; x++)
            {
                if (sameSize && after[y, x] != expected[y, x]) { mismatch++; worst = Math.Max(worst, Math.Abs(after[y, x] - expected[y, x])); }
                if (after[y, x] != before[y, x]) changed++;
            }

        Console.WriteLine($"\n   cells                : {dw * dh}");
        if (sameSize) Console.WriteLine($"   differ from source   : {mismatch}  (worst delta {worst})");
        Console.WriteLine($"   changed vs. original : {changed}");

        static (float lo, float hi, double mean) Stats(float[,] a)
        {
            float lo = float.MaxValue, hi = float.MinValue; double sum = 0; int n = 0;
            foreach (float v in a) { lo = Math.Min(lo, v); hi = Math.Max(hi, v); sum += v; n++; }
            return (lo, hi, sum / n);
        }
        var sb = Stats(before); var sa = Stats(after); var se = Stats(expected);
        Console.WriteLine($"   elevation before     : {sb.lo:F1} .. {sb.hi:F1}  mean {sb.mean:F1}");
        Console.WriteLine($"   elevation source     : {se.lo:F1} .. {se.hi:F1}  mean {se.mean:F1}");
        Console.WriteLine($"   elevation after      : {sa.lo:F1} .. {sa.hi:F1}  mean {sa.mean:F1}");
        Console.WriteLine($"   (TS2 water level is 312.5)");

        bool backupOk = report.BackupPath is not null && System.IO.File.Exists(report.BackupPath);
        Console.WriteLine($"   backup written       : {backupOk}");

        if (!sameSize)
        {
            Console.WriteLine("   (sizes differ — terrain was rescaled, so no cell-for-cell comparison)");
            if (changed > 0 && backupOk) { Console.WriteLine("\nOK — terrain was rewritten and backed up."); return 0; }
        }
        else if (mismatch == 0 && changed > 0 && backupOk) { Console.WriteLine("\nOK — terrain matches the source exactly."); return 0; }
        Console.WriteLine("\nFAIL");
        return 1;
    }
}
