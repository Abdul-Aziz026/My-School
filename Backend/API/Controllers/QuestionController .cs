using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.QuestionManagement.Commands.CreateQuestion;
using Application.Features.SchoolClassManagement.QuestionManagement.Commands.UpdateQuestion;
using Application.Features.SchoolClassManagement.QuestionManagement.DTOs;
using Application.Features.SchoolClassManagement.QuestionManagement.Queries.GetAllQuestions;
using Application.Features.SchoolClassManagement.QuestionManagement.Queries.GetQuestionsBySubject;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController : ControllerBase
{
    public readonly IMessageBus _messageBus;

    public QuestionController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpPost]
    public async Task<IActionResult> AddQuestion(CreateQuestionDto question)
    {
        var command = question.ToCreateQuestionCommand();
        var result = await _messageBus.SendAsync<CreateQuestionCommand, QuestionDto>(command);
        return Ok(result);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateQuestion(UpdateQuestionDto question)
    {
        var command = question.ToUpdateQuestionCommand();
        var result = await _messageBus.SendAsync<UpdateQuestionCommand, QuestionDto>(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuestions()
    {
        var query = new GetAllQuestionsQuery();
        var questions = await _messageBus.SendAsync<GetAllQuestionsQuery, List<QuestionDto>>(query);
        return Ok(questions);
    }

    [HttpGet("subject/{subject}")]
    public async Task<IActionResult> GetBySubject(string subject)
    {
        var query = new GetQuestionsBySubjectQuery { SubjectName = subject };
        var questions = await _messageBus.SendAsync<GetQuestionsBySubjectQuery, List<QuestionDto>>(query);
        return Ok(questions);
    }
}
