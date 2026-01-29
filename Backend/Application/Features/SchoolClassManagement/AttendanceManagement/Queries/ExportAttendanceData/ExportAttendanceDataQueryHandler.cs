using System.Linq.Expressions;
using Application.Common.Extensions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Queries.ExportAttendanceData;

public class ExportAttendanceDataQueryHandler : IRequestHandler<ExportAttendanceDataQuery, AttendanceFileResultDto>
{
    private readonly IAttendanceRepository _attendanceRepository;

    public ExportAttendanceDataQueryHandler(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task<AttendanceFileResultDto> Handle(ExportAttendanceDataQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<Attendance, bool>> filter = x => x.Date >= query.StartDate && x.Date <= query.EndDate;
        if (!string.IsNullOrEmpty(query.ClassId))
        {
            filter = filter.And(x => x.ClassId == query.ClassId);
        }

        var attendanceResponse = _attendanceRepository.GetItemsByConditionAsync(filter);
        if (query.Format.ToLowerInvariant() == "excel")
        {
            return await GenerateExcelFileAsync(attendanceResponse, query);
        }
        else if (query.Format.ToLowerInvariant() == "pdf")
        {
            return await GeneratePdfFileAsync(attendanceResponse, query);
        }
        throw new InvalidOperationException("Invalid export format");throw new InvalidOperationException("Invalid export format");
    }

    private async Task<AttendanceFileResultDto> GenerateExcelFileAsync(Task<List<Attendance>?> attendanceResponse, ExportAttendanceDataQuery query)
    {
        throw new NotImplementedException();
    }

    private async Task<AttendanceFileResultDto> GeneratePdfFileAsync(Task<List<Attendance>?> attendanceResponse, ExportAttendanceDataQuery query)
    {
        throw new NotImplementedException();
    }
}