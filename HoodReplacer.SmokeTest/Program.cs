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
        if (!dims.ContainsKey(srcCode) || !dims.ContainsKey(dstCode))
        { Console.WriteLine($"\nFAIL: unknown hood code."); return 1; }

        var (sw, sh, srcPath) = dims[srcCode];
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
        var srcPkg = SimPe.Packages.File.LoadFromFile(srcPath);
        var expected = ReadTerrain(srcPkg)!.GetTerrain(0, 0, sh, sw);
        srcPkg.ForgetUpdate(); srcPkg.Close();

        var engine = new ReplaceEngine();
        var report = engine.Run(srcPath, workPath, new ReplaceOptions { ReplaceTerrain = true });

        foreach (var l in report.Log) Console.WriteLine($"   log: {l}");
        foreach (var e in report.Errors) Console.WriteLine($"   ERR: {e}");
        Console.WriteLine($"   changed={report.Changed} saved={report.Saved} backup={report.BackupPath ?? "(none)"}");
        if (!report.Saved) { Console.WriteLine("\nFAIL: nothing was saved."); return 1; }

        // AFTER — re-read from disk, not from memory
        var reread = SimPe.Packages.File.LoadFromFile(workPath);
        var after = ReadTerrain(reread)!.GetTerrain(0, 0, dh, dw);
        reread.ForgetUpdate(); reread.Close();

        int mismatch = 0, changed = 0;
        float worst = 0;
        for (int y = 0; y < dh; y++)
            for (int x = 0; x < dw; x++)
            {
                if (after[y, x] != expected[y, x]) { mismatch++; worst = Math.Max(worst, Math.Abs(after[y, x] - expected[y, x])); }
                if (after[y, x] != before[y, x]) changed++;
            }

        Console.WriteLine($"\n   cells                : {dw * dh}");
        Console.WriteLine($"   differ from source   : {mismatch}  (worst delta {worst})");
        Console.WriteLine($"   changed vs. original : {changed}");

        bool backupOk = report.BackupPath is not null && System.IO.File.Exists(report.BackupPath);
        Console.WriteLine($"   backup written       : {backupOk}");

        if (mismatch == 0 && changed > 0 && backupOk) { Console.WriteLine("\nOK — terrain matches the source exactly."); return 0; }
        Console.WriteLine("\nFAIL");
        return 1;
    }
}
