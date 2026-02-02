using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.ExamManagement.Commands.AssignQuestions;
using Application.Features.SchoolClassManagement.ExamManagement.Commands.CreateExam;
using Application.Features.SchoolClassManagement.ExamManagement.Commands.GetExamQuestions;
using Application.Features.SchoolClassManagement.ExamManagement.Commands.PublishExam;
using Application.Features.SchoolClassManagement.ExamManagement.DTOs;
using Application.Features.SchoolClassManagement.ExamManagement.Queries.GetAllExams;
using Application.Features.SchoolClassManagement.ExamManagement.Queries.GetExamById;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamController : Controller
{
    private readonly IMessageBus _messageBus;

    public ExamController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpPost]
    public async Task<IActionResult> CreateExam(CreateExamDto exam)
    {
        var command = exam.ToCreateExamCommand();
        var result = await _messageBus.SendAsync<CreateExamCommand, string>(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllExams()
    {
        var query = new GetAllExamsQuery();
        var exams = await _messageBus.SendAsync<GetAllExamsQuery, List<ExamDto>>(query);
        return Ok(exams);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetExam(string id)
    {
        var query = new GetExamByIdQuery(id);
        var exam = await _messageBus.SendAsync<GetExamByIdQuery, ExamDto>(query);
        return exam == null ? NotFound() : Ok(exam);
    }

    // Assign questions to an exam
    [HttpPost("{examId}/assign-questions")]
    public async Task<IActionResult> AssignQuestions(string examId, [FromBody] List<string> questionIds)
    {
        var command = new AssignQuestionsCommand
        {
            ExamId = examId,
            QuestionIds = questionIds
        };
        await _messageBus.SendAsync<AssignQuestionsCommand>(command);
        return Ok("Questions assigned");
    }

    // Get questions of an exam
    [HttpGet("{examId}/exam-questions")]
    public async Task<IActionResult> GetExamQuestions(string examId)
    {
        var query = new GetExamQuestionsQuery(examId);
        var questions = await _messageBus.SendAsync<GetExamQuestionsQuery, List<ExamQuestionDto>>(query);
        return Ok(questions);
    }

    [HttpPost("{examId}/publish")]
    public async Task<IActionResult> PublishExam(string examId)
    {
        var command = new PublishExamCommand(examId);
        await _messageBus.SendAsync<PublishExamCommand>(command);
        return Ok("Exam published");
    }
}
