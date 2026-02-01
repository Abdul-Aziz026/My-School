using Application.Common.Interfaces.Publisher;
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
    public async Task<IActionResult> CreateExam(Exam exam)
    {
        //var result = await _examService.CreateExam(exam);
        return Ok(/*result*/);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllExams()
    {
        //var exams = await _examService.GetAllExams();
        return Ok(/*exams*/);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetExam(string id)
    {
        //var exam = await _examService.GetExamById(id);
        //return exam == null ? NotFound() : Ok(exam);
        return Ok();
    }

    // Assign questions to an exam
    [HttpPost("{examId}/assign-questions")]
    public async Task<IActionResult> AssignQuestions(string examId, [FromBody] List<string> questionIds)
    {
        //  await _examService.AssignQuestions(examId, questionIds);
        return Ok("Questions assigned");
    }

    // Get questions of an exam
    [HttpGet("{examId}/exam-questions")]
    public async Task<IActionResult> GetExamQuestions(string examId)
    {
        //var questions = await _examService.GetExamQuestions(examId);
        return Ok(/*questions*/);
    }

    [HttpPost("{examId}/publish")]
    public async Task<IActionResult> PublishExam(string examId)
    {
        //await _examService.PublishExam(examId);
        return Ok("Exam published");
    }
}
