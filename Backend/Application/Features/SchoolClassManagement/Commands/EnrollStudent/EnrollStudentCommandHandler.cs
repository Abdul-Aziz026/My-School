
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using Domain.Entities.JunctionEntities;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.Commands.EnrollStudent;

public class EnrollStudentCommandHandler : IRequestHandler<EnrollStudentCommand, EnrollStudentResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentRepository _studentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<EnrollStudentCommandHandler> _logger;

    public EnrollStudentCommandHandler(IEnrollmentRepository enrollmentRepository,
                                       IUnitOfWork unitOfWork,
                                       IStudentRepository studentRepository,
                                       IPaymentRepository paymentRepository,
                                       ILogger<EnrollStudentCommandHandler> logger)

    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _studentRepository = studentRepository;
        _paymentRepository = paymentRepository;
        _logger = logger;
    }
    public async Task<EnrollStudentResponseDto> Handle(EnrollStudentCommand command, CancellationToken cancellationToken)
    {
        Expression<Func<Student, bool>> filter = x => x.Id == command.StudentId;
        var student = await _studentRepository.GetItemByConditionAsync<Student>(filter);
        if (student is null)
        {
            throw new ArgumentException("Student not found");
        }

        // Start transaction (Enrollment + Payment must succeed together)
        await _unitOfWork.StartTransactionAsync();
        try
        {
            // 1. Create Enrollment
            var enrollment = new Enrollment
            {
                StudentId = command.StudentId,
                ClassId = command.ClassId,
                AcademicYear = command.AcademicYear,
                EnrollmentDate = DateTime.UtcNow,
                TuitionFee = command.TuitionFee,
                Status = EnrollMentStatus.Enrolled
            };
            await _enrollmentRepository.AddAsync(enrollment);

            _logger.LogInformation("Enrollment created: {EnrollmentId}", enrollment.Id);

            // 2. Record Initial Payment
            var payment = new Payment
            {
                StudentId = command.StudentId,
                EnrollmentId = enrollment.Id,
                Amount = command.InitialPayment,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = command.PaymentMethod,
                Status = PaymentStatus.Paid
            };
            await _paymentRepository.AddAsync(payment);
            _logger.LogInformation("Payment recorded: {PaymentId}", payment.Id);

            // Commit transaction
            await _unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Student enrolled successfully: StudentId={StudentId}, EnrollmentId={EnrollmentId}",
                command.StudentId, enrollment.Id);

            return new EnrollStudentResponseDto
            {
                Success = true,
                Message = "Student enrolled successfully",
                EnrollmentId = enrollment.Id,
                PaymentId = payment.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling student: {StudentId}", command.StudentId);
            return new EnrollStudentResponseDto {
                Success = false,
                Message = ex.Message
            };
        }
    }
}
