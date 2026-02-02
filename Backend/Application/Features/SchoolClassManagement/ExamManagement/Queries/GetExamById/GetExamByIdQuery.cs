using Application.Features.SchoolClassManagement.ExamManagement.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SchoolClassManagement.ExamManagement.Queries.GetExamById;

public class GetExamByIdQuery : IRequest<ExamDto>
{
    public string ExamId { get; set; }
    public GetExamByIdQuery(string examId)
    {
        ExamId = examId;
    }
}
