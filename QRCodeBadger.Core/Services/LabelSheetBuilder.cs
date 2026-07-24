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

        Document.Create(container =>
        {
            foreach (var pageVolunteers in pages)
            {
                container.Page(page =>
                {
                    page.Size(new PageSize((float)_options.PageWidthMm, (float)_options.PageHeightMm, Unit.Millimetre));
                    page.MarginLeft((float)_options.MarginLeftMm, Unit.Millimetre);
                    page.MarginTop((float)_options.MarginTopMm, Unit.Millimetre);
                    page.MarginRight((float)_options.MarginRightMm, Unit.Millimetre);
                    page.MarginBottom((float)_options.MarginBottomMm, Unit.Millimetre);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            for (var c = 0; c < _options.Columns; c++)
                                columns.ConstantColumn((float)_options.LabelWidthMm, Unit.Millimetre);
                        });

                        foreach (var volunteer in pageVolunteers)
                        {
                            table.Cell()
                                .Element(StyleCell)
                                .Column(column => PopulateLabel(column, volunteer));
                        }
                    });
                });
            }
        })
        .GeneratePdf(outputPdfPath);
    }

    // Styling only - Element() needs a Func<IContainer, IContainer>, so this
    // must stop short of any terminal content call (Text/Image/Column etc.),
    // which all return void rather than IContainer.
    private IContainer StyleCell(IContainer container)
    {
        return container
            .Height((float)_options.LabelHeightMm, Unit.Millimetre)
            .Padding(2);
    }

    // Content population - Column() is terminal (void), so this is a
    // separate method chained on *after* Element(StyleCell) above.
    private void PopulateLabel(ColumnDescriptor column, Volunteer volunteer)
    {
        column.Item()
            .Height((float)(_options.LabelHeightMm - _options.QrSizeMm), Unit.Millimetre)
            .AlignCenter()
            .AlignMiddle()
            .Text(volunteer.Name)
            .FontSize((float)_options.NameFontSize)
            .SemiBold();

        column.Item()
            .AlignCenter()
            .Width((float)_options.QrSizeMm, Unit.Millimetre)
            .Height((float)_options.QrSizeMm, Unit.Millimetre)
            .Image(_qrCodeGenerator.GeneratePng(volunteer.Uuid));
    }
}
