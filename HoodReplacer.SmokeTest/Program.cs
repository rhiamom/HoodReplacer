// HoodReplacer smoke test — headless harness.
//
// Stage 1: prove the vendored Filetypes DBPF layer and the Mac path discovery
// work against the real Sims 2 install before any UI exists.

using LotExpander;

namespace HoodReplacer.SmokeTest;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.WriteLine("HoodReplacer smoke test");
        Console.WriteLine(new string('-', 60));

        string? user = SimsPaths.UserFolder;
        string? hoods = SimsPaths.NeighborhoodsFolder;

        Console.WriteLine($"User folder    : {user ?? "(not found)"}");
        Console.WriteLine($"Neighborhoods  : {hoods ?? "(not found)"}");

        if (hoods is null)
        {
            Console.WriteLine();
            Console.WriteLine("FAIL: no Sims 2 user folder found.");
            return 1;
        }

        var list = NeighborhoodCatalog.List(hoods);
        Console.WriteLine($"Found {list.Count} neighborhood(s):");
        foreach (var n in list)
            Console.WriteLine($"   {n.Code,-6} {n.Name}");

        Console.WriteLine();
        Console.WriteLine("OK");
        return 0;
    }
}
