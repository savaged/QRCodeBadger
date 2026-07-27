using QRCodeBadger.Options;
using QRCodeBadger.Services;

if (args.Length < 1)
{
    Console.WriteLine("Usage: QRCodeBadger <input.xlsx|input.csv> [output.pdf]");
    Console.WriteLine("  input file  - .xlsx or .csv with Name (col A) and UUID (col B), header row 1");
    Console.WriteLine("  output.pdf  - defaults to labels.pdf in the current folder");
    return 1;
}

var inputPath = args[0];
var outputPath = args.Length > 1 ? args[1] : "labels.pdf";

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

var options = new LabelSheetOptions
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

ILabelSheetBuilder builder = new LabelSheetBuilder(options, qrCodeGenerator);

var volunteers = reader.ReadVolunteers(inputPath);

if (volunteers.Count == 0)
{
    Console.Error.WriteLine("No volunteer rows found - check the spreadsheet layout.");
    return 1;
}

builder.Build(volunteers, outputPath);

Console.WriteLine($"Generated {volunteers.Count} labels -> {outputPath}");
return 0;
