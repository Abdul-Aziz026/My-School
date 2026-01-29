namespace Application.Common.Interfaces.Services;

public interface IDocumentService
{
    byte[] GenerateExcel<T>(IEnumerable<T> data, string sheetName = "Report") where T : class;
    byte[] GeneratePdf<T>(IEnumerable<T> data, string title = "Report") where T: class;
}