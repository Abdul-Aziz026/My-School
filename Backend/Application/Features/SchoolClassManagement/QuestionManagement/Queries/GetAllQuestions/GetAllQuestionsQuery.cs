
using Application.Features.SchoolClassManagement.QuestionManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.QuestionManagement.Queries.GetAllQuestions;

public class GetAllQuestionsQuery : IRequest<List<QuestionDto>>
{
}
