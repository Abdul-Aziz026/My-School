
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using Domain.Entities;
using MediatR;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.StudentManagement.Queries.GetStudentResult;

public class GetStudentResultQueryHandler : IRequestHandler<GetStudentResultQuery, ExamResultDto>
{
    private readonly IExamRepository _examRepository;
    private readonly IStudentRepository _studentRepository;
    public GetStudentResultQueryHandler(IExamRepository examRepository, IStudentRepository studentRepository)
    {
        _examRepository = examRepository;
        _studentRepository = studentRepository;
    }

    public async Task<ExamResultDto> Handle(GetStudentResultQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<ExamResult, bool>> filter = x => x.ExamId == request.ExamId
                                                && x.StudentId == request.StudentId;

        var result = await _examRepository.GetItemByConditionAsync(filter);
        if (result == null)
        {
            throw new NotFoundException("return not found");
        }
        return new ExamResultDto
        {
            ExamId = result.ExamId,
            StudentId = result.StudentId,
            TotalMarks = result.TotalMarks,
            ObtainedMarks = result.ObtainedMarks,
            Percentage = result.Percentage,
            IsPassed = result.IsPassed,
            EvaluatedAt = result.EvaluatedAt
        };
    }
}
