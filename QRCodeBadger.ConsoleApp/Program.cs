using QRCodeBadger.Options;
using QRCodeBadger.Services;

if (args.Length < 1)
{
    Console.WriteLine("Usage: QRCodeBadger <input.xlsx|input.csv> [output.pdf] [layout number]");
    Console.WriteLine("  input file  - .xlsx or .csv with Name (col A) and UUID (col B), header row 1");
    Console.WriteLine("  output.pdf  - defaults to labels.pdf in the current folder");
    Console.WriteLine("  layout # - defaults to option 1, A4-60 (i.e. 6 columns by 10 rows). Option 2 is A4-24.");
    return 1;
}

var inputPath = args[0];
var outputPath = args.Length > 1 ? args[1] : "labels.pdf";
var layoutOption = 0;
if (args.Length > 2 && int.TryParse(args[2], out int i) && i == 2)
    layoutOption = 1;

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"File not found: {inputPath}");
    return 1;
}

ISpreadsheetReader reader;
try
{
    reader = SpreadsheetReaderFactory.Create(inputPath);
}
catch (NotSupportedException ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}

IQrCodeGenerator qrCodeGenerator = new QrCodeGenerator();

var a4_60 = new LabelSheetOptions
{
    Columns = 6,
    Rows = 10,
    LabelWidthMm = 30,
    LabelHeightMm = 25,
    MarginLeftMm = 15,
    MarginTopMm = 16,
    MarginRightMm = 15,
    MarginBottomMm = 16.4,
    QrSizeMm = 15,
    NameFontSize = 6
};
var a4_24 = new LabelSheetOptions
{
    Columns = 3,
    Rows = 8,
    LabelWidthMm = 70,
    LabelHeightMm = 36.5,
    MarginLeftMm = 0,
    MarginTopMm = 4,
    MarginRightMm = 0,
    MarginBottomMm = 2,
    QrSizeMm = 26,
    NameFontSize = 8
};
var options = new List<LabelSheetOptions> { a4_60, a4_24 };

ILabelSheetBuilder builder = new LabelSheetBuilder(options[layoutOption], qrCodeGenerator);

var volunteers = reader.ReadVolunteers(inputPath);

if (volunteers.Count == 0)
{
    Console.Error.WriteLine("No volunteer rows found - check the spreadsheet layout.");
    return 1;
}

builder.Build(volunteers, outputPath);

Console.WriteLine($"Generated {volunteers.Count} labels -> {outputPath}");
return 0;
