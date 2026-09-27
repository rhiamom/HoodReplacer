/***************************************************************************
 *   HoodReplacer for Mac — main window.
 *   The replace logic is Mootilda's, in HoodReplacer.Engine; this only
 *   collects choices and answers the engine's two callbacks.
 ***************************************************************************/

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using HoodReplace;
using LotExpander;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoodReplacer.App.Views;

public partial class MainWindow : Window
{
    private sealed record Entry(string Label, string Path, bool IsSc4)
    {
        public override string ToString() => Label;
    }

    private readonly List<Entry> _sources = new();
    private readonly List<Entry> _targets = new();

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        Opened += (_, _) => LoadNeighborhoods();
        this.Get<Button>("BrowseSrc").Click += BrowseSrcClick;
        this.Get<Button>("BrowseDst").Click += BrowseDstClick;
        this.Get<Button>("ExitButton").Click += (_, _) => Close();
        this.Get<Button>("CopyButton").Click += CopyClick;
    }

    private void Say(string message) => this.Get<TextBlock>("Status").Text = message;

    private void LoadNeighborhoods()
    {
        string? folder = SimsPaths.NeighborhoodsFolder;
        if (folder is null)
        {
            Say("No Sims 2 user folder found.");
            return;
        }

        _sources.Clear();
        _targets.Clear();
        foreach (var n in NeighborhoodCatalog.List(folder))
        {
            _sources.Add(new Entry($"{n.Name}  ({n.Code})", n.MainPackagePath, false));
            _targets.Add(new Entry($"{n.Name}  ({n.Code})", n.MainPackagePath, false));
        }
        this.Get<ListBox>("ListSrc").ItemsSource = null;
        this.Get<ListBox>("ListSrc").ItemsSource = _sources;
        this.Get<ListBox>("ListDst").ItemsSource = null;
        this.Get<ListBox>("ListDst").ItemsSource = _targets;
        Say($"{_targets.Count} neighborhoods.");
    }

    private async void BrowseSrcClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose a SimCity 4 terrain or a neighborhood package",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Terrain / neighborhood") { Patterns = new[] { "*.sc4", "*.SC4", "*.package" } }
            }
        });
        var f = files.FirstOrDefault();
        if (f is null) return;
        string path = f.Path.LocalPath;
        bool sc4 = string.Equals(Path.GetExtension(path), ".sc4", StringComparison.OrdinalIgnoreCase);
        var entry = new Entry(Path.GetFileNameWithoutExtension(path) + (sc4 ? "  (SC4)" : ""), path, sc4);
        _sources.Insert(0, entry);
        var lb = this.Get<ListBox>("ListSrc");
        lb.ItemsSource = null;
        lb.ItemsSource = _sources;
        lb.SelectedItem = entry;
    }

    private async void BrowseDstClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose the neighborhood package to change",
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("Neighborhood package") { Patterns = new[] { "*.package" } } }
        });
        var f = files.FirstOrDefault();
        if (f is null) return;
        string path = f.Path.LocalPath;
        var entry = new Entry(Path.GetFileNameWithoutExtension(path), path, false);
        _targets.Insert(0, entry);
        var lb = this.Get<ListBox>("ListDst");
        lb.ItemsSource = null;
        lb.ItemsSource = _targets;
        lb.SelectedItem = entry;
    }

    private ReplaceOptions CollectOptions() => new()
    {
        ReplaceTerrain     = this.Get<CheckBox>("ReplTerrain").IsChecked == true,
        ReplaceRoads       = this.Get<CheckBox>("ReplRoads").IsChecked == true,
        ReplaceBridges     = this.Get<CheckBox>("ReplBridges").IsChecked == true,
        ReplaceTrees       = this.Get<CheckBox>("ReplTrees").IsChecked == true,
        ReplaceDecorations = this.Get<CheckBox>("ReplDeco").IsChecked == true,
        FixRoads           = this.Get<CheckBox>("FixRoads").IsChecked == true,
        FixBridges         = this.Get<CheckBox>("FixBridges").IsChecked == true,
        FixTrees           = this.Get<CheckBox>("FixTrees").IsChecked == true,
        FixDecorations     = this.Get<CheckBox>("FixDeco").IsChecked == true,
        FixLots            = this.Get<CheckBox>("FixLots").IsChecked == true,
        DeleteRoads        = this.Get<CheckBox>("DelRoads").IsChecked == true,
        DeleteBridges      = this.Get<CheckBox>("DelBridges").IsChecked == true,
        DeleteTrees        = this.Get<CheckBox>("DelTrees").IsChecked == true,
        DeleteDecorations  = this.Get<CheckBox>("DelDeco").IsChecked == true,
        VersionedBackups   = this.Get<CheckBox>("MultiBackup").IsChecked == true,
    };

    private async void CopyClick(object? sender, RoutedEventArgs e)
    {
        if (this.Get<ListBox>("ListSrc").SelectedItem is not Entry src ||
            this.Get<ListBox>("ListDst").SelectedItem is not Entry dst)
        {
            await Dialogs.Message(this, "Choose both", "Pick a source and a destination first.");
            return;
        }
        if (string.Equals(src.Path, dst.Path, StringComparison.OrdinalIgnoreCase))
        {
            await Dialogs.Message(this, "Same neighborhood", "The source and destination are the same file.");
            return;
        }

        string name = Path.GetFileName(dst.Path);
        if (!await Dialogs.Confirm(this, "Replace terrain?",
                $"This rewrites {name}.\n\nA .bkp backup is written next to it first, but that backup is " +
                "overwritten on every run — keep your own copy of anything you cannot lose."))
            return;

        var engine = new ReplaceEngine();
        engine.ConfirmHandler = msg => Dialogs.ConfirmBlocking(this, "Terrain sizes differ", msg);
        engine.ResizeHandler = req => new ResizeChoice
        {
            ResizeToFit = true,
            ScaleElevation = false,
            PercentElevation = req.PercentElevation,
        };

        Say("Working…");
        this.Get<Button>("CopyButton").IsEnabled = false;
        ReplaceReport report;
        try
        {
            var options = CollectOptions();
            report = await Task.Run(() => engine.Run(src.Path, dst.Path, options));
        }
        finally
        {
            this.Get<Button>("CopyButton").IsEnabled = true;
        }

        if (report.Errors.Count > 0)
        {
            Say("Failed.");
            await Dialogs.Message(this, "Copy failed", string.Join("\n\n", report.Errors));
            return;
        }
        if (!report.Saved)
        {
            Say("Nothing changed.");
            await Dialogs.Message(this, "Nothing changed", "No changes were made.");
            return;
        }

        Say($"Done — backup at {Path.GetFileName(report.BackupPath ?? "")}");
        await Dialogs.Message(this, "Copy complete",
            $"{name} was updated.\n\nBackup: {report.BackupPath}");
    }
}
