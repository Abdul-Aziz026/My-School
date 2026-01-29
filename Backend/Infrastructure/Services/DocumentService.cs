using System.Reflection.Metadata;
using Application.Common.Interfaces.Services;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Runtime.InteropServices.JavaScript;

namespace Infrastructure.Services;

public class DocumentService : IDocumentService
{
    public byte[] GenerateExcel<T>(IEnumerable<T> list, string sheetName = "Report") where T : class
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
        int row = 2, col = 0;
        foreach (var item in list)
        {
            col = 1;
            foreach(var c in item)
            {
                var cell = workSheet.Cell(row, col);
                cell.Value = item[j];
                ++col;
            }
            row++;
        }

        workSheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GeneratePdf<T>(IEnumerable<T> data, string title = "Report") where T : class
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text(title).FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                page.Content().Table(table =>
                {
                    var props = typeof(T).GetProperties();
                    table.ColumnsDefinition(columns => {
                        foreach (var p in props) columns.RelativeColumn();
                    });

                    // Table Headers
                    foreach (var prop in props)
                        table.Cell().Background(Colors.Grey.Lighten3).Text(prop.Name).SemiBold();

                    // Table Data
                    foreach (var item in data)
                    {
                        foreach (var prop in props)
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Text(prop.GetValue(item)?.ToString());
                    }
                });
            });
        }).GeneratePdf();
    }
}