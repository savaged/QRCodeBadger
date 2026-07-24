namespace QRCodeBadger.Services;

public interface IQrCodeGenerator
{
    /// <summary>Generates a QR code as PNG bytes for the given payload.</summary>
    byte[] GeneratePng(string payload);
}
