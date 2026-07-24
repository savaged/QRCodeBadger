using QRCodeBadger.Models;

namespace QRCodeBadger.Services;

public interface ISpreadsheetReader
{
    IReadOnlyList<Volunteer> ReadVolunteers(string filePath);
}
