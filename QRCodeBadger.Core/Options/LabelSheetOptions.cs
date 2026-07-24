namespace QRCodeBadger.Options;

/// <summary>
/// All measurements are in millimetres. Defaults below roughly match a
/// common 21-per-sheet A4 label layout (e.g. Avery L7160/L7163 style).
/// Adjust to match whatever label stock you're printing onto.
/// </summary>
public sealed class LabelSheetOptions
{
    public double PageWidthMm { get; init; } = 210;
    public double PageHeightMm { get; init; } = 297;

    public double MarginLeftMm { get; init; } = 6.5;
    public double MarginTopMm { get; init; } = 15.0;
    public double MarginRightMm { get; init; } = 6.5;
    public double MarginBottomMm { get; init; } = 15.0;

    public double LabelWidthMm { get; init; } = 63.5;
    public double LabelHeightMm { get; init; } = 38.1;

    public int Columns { get; init; } = 3;
    public int Rows { get; init; } = 7;

    public double QrSizeMm { get; init; } = 20;
    public double NameFontSize { get; init; } = 11;
}
