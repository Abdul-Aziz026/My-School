
using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Contracts.Events;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.ExamManagement.Commands.PublishExam;

public class PublishExamCommandHandler : IRequestHandler<PublishExamCommand>
{
    private readonly IMessageBus _messageBus;
    private readonly IExamRepository _examRepository;
    public PublishExamCommandHandler(IExamRepository examRepository,
                                    IMessageBus messageBus)
    {
        _examRepository = examRepository;
        _messageBus = messageBus;
    }

    public async Task Handle(PublishExamCommand request, CancellationToken cancellationToken)
    {
        var exam = await _examRepository.GetByIdAsync<Exam>(request.ExamId);
        if (exam == null) {
            throw new Exception("Exam not found");
        }
        if (exam.IsPublished) {
            return; // Exam is already published, no action needed
        }

        exam.IsPublished = true;
        exam.UpdatedAt = DateTime.UtcNow;
        await _examRepository.UpdateAsync(exam);

        // Additional logic such as notifying students can be added here
        Expression<Func<Student, bool>> filter = s => s.ClassId == exam.ClassId
                                                     && s.AcademicYear == exam.AcademicYear;
        var students = await _examRepository.GetItemsByConditionAsync(filter);
        // Notify students about the published exam via email and in-app notifications
        var subject = "New Exam Published: " + exam.ExamName;
        foreach (var student in students)
        {
            var body = $"Dear {student.Name},\n\n" +
                       $"We are pleased to inform you that a new exam '{exam.ExamName}' has been published for your class.\n" +
                       $"Please check the exam schedule and prepare accordingly.\n\n" +
                       $"Best regards,\n" +
                       $"School Administration";
            var emailSendCommand = new SendEmailCommand()
            {
                ToMail = student.Email,
                Name = student.Name,
                Subject = subject,
                Body = body,
            };
            await _messageBus.PublishAsync(emailSendCommand);
            // in-app notification logic can be added here
        }
    }
}
