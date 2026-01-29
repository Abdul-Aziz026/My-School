namespace Application.Common.Interfaces.Services;

public interface IDocumentService
{
    byte[] GenerateExcel<T>(List<T> data, string sheetName = "Report") where T : class;
    byte[] GeneratePdf<T>(List<T> data, string title = "Report") where T: class;
}