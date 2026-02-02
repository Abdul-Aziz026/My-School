using Application.Features.SchoolClassManagement.ExamManagement.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SchoolClassManagement.ExamManagement.Queries.GetAllExams;

public class GetAllExamsQuery : IRequest<List<ExamDto>>
{
}
