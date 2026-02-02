using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SchoolClassManagement.ExamManagement.Commands.CreateExam;

public class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, string>
{
    private readonly IExamRepository _examRepository;
    public CreateExamCommandHandler(IExamRepository examRepository)
    {
        _examRepository = examRepository;
    }

    public async Task<string> Handle(CreateExamCommand command, CancellationToken cancellationToken)
    {
        var exam = new Exam
        {
            ClassId = command.ClassId,
            SubjectName = command.SubjectName,
            ExamName = command.ExamName,
            ExamType = command.ExamType,
            ExamDate = command.ExamDate,
            StartTime = command.StartTime,
            EndTime = command.EndTime,
            Duration = command.Duration,
            TotalMarks = command.TotalMarks,
            PassingMarks = command.PassingMarks
        };
        await _examRepository.AddAsync(exam);
        return exam.Id;
    }
}
