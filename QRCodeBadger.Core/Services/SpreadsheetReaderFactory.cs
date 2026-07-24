namespace QRCodeBadger.Services;

/// <summary>
/// Picks an ISpreadsheetReader implementation based on the input file's extension.
/// </summary>
public static class SpreadsheetReaderFactory
{
    public static ISpreadsheetReader Create(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        return extension switch
        {
            ".csv" => new CsvSpreadsheetReader(),
            ".xlsx" => new XlsxSpreadsheetReader(),
            _ => throw new NotSupportedException(
                $"Unsupported file type '{extension}'. Supported types: .xlsx, .csv")
        };
    }
}
