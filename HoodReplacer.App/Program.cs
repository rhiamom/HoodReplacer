/***************************************************************************
 *   HoodReplacer for Mac                                                  *
 *   HoodReplace © 2008-2010 Mootilda                                      *
 *   macOS port © 2026 GramzeSweatshop (rhiamom@mac.com)                   *
 *   GPL v2 or later.                                                      *
 ***************************************************************************/

using Avalonia;
using System;

namespace HoodReplacer.App;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
