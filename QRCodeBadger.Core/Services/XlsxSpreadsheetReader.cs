using ClosedXML.Excel;
using QRCodeBadger.Models;

namespace QRCodeBadger.Services;

/// <summary>
/// Reads Name/UUID pairs from the first worksheet of an .xlsx file.
/// Assumes column A = Name, column B = UUID, with a header row.
/// </summary>
public sealed class XlsxSpreadsheetReader : ISpreadsheetReader
{
    private readonly int _nameColumn;
    private readonly int _uuidColumn;
    private readonly int _headerRowCount;

    public XlsxSpreadsheetReader(int nameColumn = 1, int uuidColumn = 2, int headerRowCount = 1)
    {
        _nameColumn = nameColumn;
        _uuidColumn = uuidColumn;
        _headerRowCount = headerRowCount;
    }

    public IReadOnlyList<Volunteer> ReadVolunteers(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1);

        var volunteers = new List<Volunteer>();
        var row = _headerRowCount + 1;

        while (true)
        {
            var name = worksheet.Cell(row, _nameColumn).GetString().Trim();
            if (string.IsNullOrEmpty(name))
                break;

            var uuid = worksheet.Cell(row, _uuidColumn).GetString().Trim();
            volunteers.Add(new Volunteer(name, uuid));
            row++;
        }

        return volunteers;
    }
}
