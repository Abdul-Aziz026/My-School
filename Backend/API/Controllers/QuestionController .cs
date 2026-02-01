using Application.Common.Interfaces.Publisher;
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
    public async Task<IActionResult> AddQuestion(Question question)
    {
        //var result = await _questionService.AddQuestion(question);
        return Ok(/*result*/);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuestions()
    {
        //var questions = await _questionService.GetAllQuestions();
        return Ok(/*questions*/);
    }

    [HttpGet("subject/{subject}")]
    public async Task<IActionResult> GetBySubject(string subject)
    {
        //var questions = await _questionService.GetQuestionsBySubject(subject);
        return Ok(/*questions*/);
    }

}
