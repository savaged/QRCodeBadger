using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QRCodeBadger.Models;
using QRCodeBadger.Options;

namespace QRCodeBadger.Services;

public sealed class LabelSheetBuilder : ILabelSheetBuilder
{
    private readonly LabelSheetOptions _options;
    private readonly IQrCodeGenerator _qrCodeGenerator;

    public LabelSheetBuilder(LabelSheetOptions options, IQrCodeGenerator qrCodeGenerator)
    {
        _options = options;
        _qrCodeGenerator = qrCodeGenerator;
    }

    public void Build(IReadOnlyList<Volunteer> volunteers, string outputPdfPath)
    {
        // QuestPDF's Community license is free for individuals/small companies.
        // See https://www.questpdf.com/license/ if this doesn't apply to you.
        QuestPDF.Settings.License = LicenseType.Community;

        var perPage = _options.Columns * _options.Rows;

        var pages = volunteers
            .Select((volunteer, index) => (volunteer, index))
            .GroupBy(x => x.index / perPage)
            .OrderBy(g => g.Key)
            .Select(g => g.Select(x => x.volunteer).ToList())
            .ToList();

        Document.Create(document =>
        {
            foreach (var pageVolunteers in pages)
            {
                document.Page(page =>
                {
                    page.Size(new PageSize((float)_options.PageWidthMm, (float)_options.PageHeightMm, Unit.Millimetre));
                    page.MarginLeft((float)_options.MarginLeftMm, Unit.Millimetre);
                    page.MarginTop((float)_options.MarginTopMm, Unit.Millimetre);
                    page.MarginRight((float)_options.MarginRightMm, Unit.Millimetre);
                    page.MarginBottom((float)_options.MarginBottomMm, Unit.Millimetre);

                    page.Content().Column(pageColumn =>
                    {
                        foreach (var rowOfVolunteers in pageVolunteers.Chunk(_options.Columns))
                        {
                            pageColumn.Item().Row(row =>
                            {
                                foreach (var volunteer in rowOfVolunteers)
                                    row.ConstantItem((float)_options.LabelWidthMm, Unit.Millimetre)
                                        .Element(StyleLabel)
                                        .Column(column => PopulateLabel(column, volunteer));
                            });
                        }
                    });
                });
            }
        })
        .GeneratePdf(outputPdfPath);
    }

    private IContainer StyleLabel(IContainer container) =>
        container
            .Height((float)_options.LabelHeightMm, Unit.Millimetre)
            .Padding(2);

    private void PopulateLabel(ColumnDescriptor column, Volunteer volunteer)
    {
        column.Item()
            .AlignCenter()
            .Text(volunteer.Name)
            .FontSize((float)_options.NameFontSize)
            .SemiBold();

        column.Item()
            .PaddingTop(2)
            .AlignCenter()
            .Width((float)_options.QrSizeMm, Unit.Millimetre)
            .Height((float)_options.QrSizeMm, Unit.Millimetre)
            .Image(_qrCodeGenerator.GeneratePng(volunteer.Uuid));
    }
}
