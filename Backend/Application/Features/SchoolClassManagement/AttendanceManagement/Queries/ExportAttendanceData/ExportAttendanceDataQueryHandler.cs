using System.Linq.Expressions;
using Application.Common.Extensions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Queries.ExportAttendanceData;

public class ExportAttendanceDataQueryHandler : IRequestHandler<ExportAttendanceDataQuery, AttendanceFileResultDto>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IDocumentService _documentService;

    public ExportAttendanceDataQueryHandler(IAttendanceRepository attendanceRepository,
                                            IDocumentService documentService)
    {
        _attendanceRepository = attendanceRepository;
        _documentService = documentService;
    }

    public async Task<AttendanceFileResultDto> Handle(ExportAttendanceDataQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<Attendance, bool>> filter = x => x.Date >= query.StartDate && x.Date <= query.EndDate;
        if (!string.IsNullOrEmpty(query.ClassId))
        {
            filter = filter.And(x => x.ClassId == query.ClassId);
        }

        var attendanceResponse = await _attendanceRepository.GetItemsByConditionAsync(filter);
        attendanceResponse ??= new List<Attendance>();

        if (query.Format.ToLowerInvariant() == "excel")
        {
            return await GenerateExcelFileAsync(attendanceResponse);
        }
        else if (query.Format.ToLowerInvariant() == "pdf")
        {
            return await GeneratePdfFileAsync(attendanceResponse, query);
        }

        throw new InvalidOperationException("Invalid export format");
    }

    private async Task<AttendanceFileResultDto> GenerateExcelFileAsync(List<Attendance> attendanceResponse)
    {
        var excelResponse = _documentService.GenerateExcel<Attendance>(attendanceResponse, "Attendance Report");
        return new AttendanceFileResultDto
        {
            ContentType = "Excel",
            FileName = "Attendance Summery",
            FileContent = excelResponse
        };
    }

    private async Task<AttendanceFileResultDto> GeneratePdfFileAsync(List<Attendance> attendanceResponse, ExportAttendanceDataQuery query)
    {
        var pdfResponse = _documentService.GeneratePdf<Attendance>(attendanceResponse, "Attendance Report");
        return new AttendanceFileResultDto
        {
            ContentType = "PDF",
            FileName = "Attendance Summery",
            FileContent = pdfResponse
        };
    }
}