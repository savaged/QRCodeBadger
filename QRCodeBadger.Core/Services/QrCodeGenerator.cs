using QRCoder;

namespace QRCodeBadger.Services;

public sealed class QrCodeGenerator : IQrCodeGenerator
{
    public byte[] GeneratePng(string payload)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
        var pngQrCode = new PngByteQRCode(qrCodeData);

        // pixelsPerModule = 20 gives a crisp image at typical label print sizes
        return pngQrCode.GetGraphic(pixelsPerModule: 20);
    }
}
