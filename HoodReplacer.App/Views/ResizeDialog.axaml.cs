/***************************************************************************
 *   The Avalonia stand-in for Mootilda's ResizeTerrain form.
 *   Same two mutually exclusive modes, same constraints, same wording:
 *     - "Resize to fit" off -> place the terrain at Top/Left
 *     - "Resize to fit" on  -> scale it, optionally scaling elevation too
 *   Ported from WinForms/ResizeTerrain.cs; her explanation strings are kept
 *   verbatim. She raises it from inside ReplaceNHTG, so the engine asks for
 *   it through ResizeHandler and this answers.
 ***************************************************************************/

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using HoodReplace;
using System;
using System.Threading.Tasks;

namespace HoodReplacer.App.Views;

public partial class ResizeDialog : Window
{
    // Her strings, unchanged.
    private const string ExplIncrease = "The source terrain is smaller than the destination terrain.  ";
    private const string ExplDecrease = "The source terrain is larger than the destination terrain.  ";
    private const string ExplCoords   = "Please select the starting coordinates for the copy.";
    private const string ExplProceed  = "Do you want to scale the elevation as well?";
    private const string ExplScale    = "Please select the amount to scale the elevation.";

    private string _expl = "The terrain sizes do not match.  ";
    private ResizeChoice? _result;

    public ResizeDialog() => AvaloniaXamlLoader.Load(this);

    private ResizeDialog(ResizeRequest r) : this()
    {
        var top   = this.Get<NumericUpDown>("TopUpDown");
        var left  = this.Get<NumericUpDown>("LeftUpDown");
        var elev  = this.Get<NumericUpDown>("ElevationUpDown");
        var fit   = this.Get<CheckBox>("ResizeToFit");
        var scale = this.Get<CheckBox>("ScaleElevation");

        this.Get<TextBlock>("Sizes").Text =
            $"Source {r.SrcWidth} x {r.SrcHeight}, destination {r.DstWidth} x {r.DstHeight}.";

        if (r.SrcHeight < r.DstHeight)
        {
            _expl = ExplIncrease;
            top.Maximum  = r.DstHeight - r.SrcHeight;
            left.Maximum = r.DstWidth  - r.SrcWidth;
            elev.Minimum = 101;
            elev.Maximum = (r.DstWidth * 100) / r.SrcWidth;
            elev.Value   = elev.Maximum;
            if (r.DstHeight % r.SrcHeight != 0 || r.DstWidth % r.SrcWidth != 0)
                fit.IsEnabled = false;
        }
        else
        {
            _expl = ExplDecrease;
            top.Maximum  = r.SrcHeight - r.DstHeight;
            left.Maximum = r.SrcWidth  - r.DstWidth;
            elev.Minimum = (r.DstWidth * 100) / r.SrcWidth;
            elev.Maximum = 99;
            elev.Value   = elev.Minimum;
            if (r.SrcHeight % r.DstHeight != 0 || r.SrcWidth % r.DstWidth != 0)
                fit.IsEnabled = false;
        }

        // Her HoodReplace.cs overrides the default to 121% for an SC4 source
        // growing to a larger hood; the engine passes that through.
        if (r.PercentElevation >= elev.Minimum && r.PercentElevation <= elev.Maximum)
            elev.Value = r.PercentElevation;

        top.Value  = top.Maximum  / 2;
        left.Value = left.Maximum / 2;

        this.Get<TextBlock>("Expl").Text = _expl + ExplCoords;

        fit.IsCheckedChanged += (_, _) => FitChanged();
        scale.IsCheckedChanged += (_, _) => ScaleChanged();
        this.Get<Button>("ButtonCancel").Click += (_, _) => { _result = null; Close(); };
        this.Get<Button>("ButtonOK").Click += (_, _) =>
        {
            _result = new ResizeChoice
            {
                ResizeToFit      = fit.IsChecked == true,
                ScaleElevation   = scale.IsChecked == true,
                PercentElevation = (int)(elev.Value ?? r.PercentElevation),
                Top              = (int)(top.Value ?? 0),
                Left             = (int)(left.Value ?? 0),
            };
            Close();
        };
    }

    private void FitChanged()
    {
        bool fit = this.Get<CheckBox>("ResizeToFit").IsChecked == true;
        this.Get<Grid>("ResizeGroup").IsVisible = !fit;
        this.Get<StackPanel>("ScalePanel").IsVisible = fit;
        this.Get<TextBlock>("Expl").Text = _expl + (fit ? ExplProceed : ExplCoords);
        if (fit) ScaleChanged();
    }

    private void ScaleChanged()
    {
        bool scale = this.Get<CheckBox>("ScaleElevation").IsChecked == true;
        this.Get<Grid>("ScaleGroup").IsVisible = scale;
        this.Get<TextBlock>("Expl").Text = _expl + (scale ? ExplScale : ExplProceed);
    }

    /// <summary>
    /// The engine asks from a worker thread, so hop to the UI thread and
    /// wait. Null is Cancel, which aborts the copy exactly as her
    /// DialogResult != OK did.
    /// </summary>
    public static ResizeChoice? Ask(Window owner, ResizeRequest request) =>
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var dlg = new ResizeDialog(request);
            await dlg.ShowDialog(owner);
            return dlg._result;
        }).GetAwaiter().GetResult();
}
