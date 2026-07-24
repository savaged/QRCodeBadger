using QRCodeBadger.Models;

namespace QRCodeBadger.Services;

public interface ILabelSheetBuilder
{
    void Build(IReadOnlyList<Volunteer> volunteers, string outputPdfPath);
}
