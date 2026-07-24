using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using QRCodeBadger.Models;

namespace QRCodeBadger.Services;

/// <summary>
/// Reads Name/UUID pairs from a CSV file.
/// Assumes column A = Name, column B = UUID, with a header row.
/// Column order/names can be overridden via the constructor.
/// </summary>
public sealed class CsvSpreadsheetReader : ISpreadsheetReader
{
    private readonly string? _nameHeader;
    private readonly string? _uuidHeader;

    /// <param name="nameHeader">
    /// If set, columns are matched by this header name instead of position.
    /// </param>
    /// <param name="uuidHeader">
    /// If set, columns are matched by this header name instead of position.
    /// </param>
    public CsvSpreadsheetReader(string? nameHeader = null, string? uuidHeader = null)
    {
        _nameHeader = nameHeader;
        _uuidHeader = uuidHeader;
    }

    public IReadOnlyList<Volunteer> ReadVolunteers(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            TrimOptions = TrimOptions.Trim
        };

        using var streamReader = new StreamReader(filePath);
        using var csv = new CsvReader(streamReader, config);

        var volunteers = new List<Volunteer>();

        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            var name = _nameHeader is not null
                ? csv.GetField(_nameHeader)
                : csv.GetField(0);

            var uuid = _uuidHeader is not null
                ? csv.GetField(_uuidHeader)
                : csv.GetField(1);

            name = name?.Trim() ?? string.Empty;
            uuid = uuid?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(name))
                continue;

            volunteers.Add(new Volunteer(name, uuid));
        }

        return volunteers;
    }
}
