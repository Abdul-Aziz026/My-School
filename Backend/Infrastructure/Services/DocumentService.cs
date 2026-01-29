using Application.Common.Interfaces.Services;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Infrastructure.Services;

public class DocumentService : IDocumentService
{
    public byte[] GenerateExcel<T>(List<T> data, string sheetName = "Report") where T : class
    {
        using var workbook = new XLWorkbook();
        var workSheet = workbook.Worksheets.Add(sheetName);

        // Uses Reflection to automatically create headers
        var properties = typeof(T).GetProperties();

        // Generate Headers
        for (int i = 0; i < properties.Length; i++)
        {
            var cell = workSheet.Cell(1, i + 1);
            cell.Value = properties[i].Name;
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.AliceBlue;
        }

        // Generate Data Rows
        var dataList = data.ToList();
        for (int row = 0; row < dataList.Count; ++row)
        {
            for(int col = 0; col < properties.Length; ++col)
            {
                var value = properties[col].GetValue(dataList[row]);
                workSheet.Cell(row + 2, col + 1).Value = value?.ToString() ?? string.Empty;
            }
        }

        workSheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GeneratePdf<T>(List<T> data, string title = "Report") where T : class
    {
        if (data == null || !data.Any())
        {
            throw new ArgumentException("Data cannot be null or empty");
        }
        var properties = typeof(T).GetProperties().Where(p => p.CanRead).ToList();
        if (!properties.Any())
        {
            throw new InvalidOperationException($"Type {typeof(T).Name} has no displayable properties.");
        }

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(PdfHelper.DefaultMargin);
                // Header
                page.Header().Column(column =>
                {
                    column.Item().Text(title)
                        .FontSize(PdfHelper.HeaderFontSize)
                        .SemiBold()
                        .FontColor(Colors.Blue.Medium);
                    column.Item().Text($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm}")
                        .FontSize(PdfHelper.SubHeaderFontSize)
                        .Italic()
                        .FontColor(Colors.Grey.Medium);
                });

                // Content
                page.Content().PaddingTop(10).Table(table =>
                {
                    // Define columns
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var _ in properties)
                        {
                            columns.RelativeColumn();
                        }
                    });

                    // Table Headers
                    foreach (var prop in properties)
                    {
                        table.Cell().Background(Colors.Grey.Lighten3)
                            .Padding(5)
                            .Text(prop.Name)
                            .FontSize(PdfHelper.DefaultMargin)
                            .SemiBold();
                    }

                    // Table Data
                    var dataList = data.ToList();
                    for (int row = 0; row < dataList.Count; ++row)
                    {
                        var bgColor = row % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                        for (int col = 0; col < properties.Count; ++col)
                        {
                            var value = properties[col].GetValue(dataList[row]);
                            var formattedValue = FormatValue(value);

                            table.Cell()
                                .Background(bgColor)
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Lighten4)
                                .Padding(5)
                                .Text(formattedValue)
                                .FontSize(PdfHelper.TableCellFontSize);
                        }
                    }
                });
            });
        }).GeneratePdf();
    }

    private string FormatValue(object value)
    {
        if (value == null) return "";
        return value switch
        {
            DateTime date => date.ToString("yyyy-MM-dd"),
            DateTimeOffset dateOffset => dateOffset.ToString("yyyy-MM-dd"),
            decimal dec => dec.ToString("N2"),
            double dbl => dbl.ToString("N2"),
            float flt => flt.ToString("N2"),
            bool b => b ? "Yes" : "No",
            _ => value.ToString() ?? string.Empty
        };
    }
}

public static class PdfHelper
{
    public static int DefaultMargin = 40;
    public static int HeaderFontSize = 24;
    public static int SubHeaderFontSize = 10;
    public static int TableHeaderFontSize = 11;
    public static int TableCellFontSize = 10;
    public static int MaxCellLength = 100;
}