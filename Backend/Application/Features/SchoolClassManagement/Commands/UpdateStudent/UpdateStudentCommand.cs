using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.UpdateStudent;

public class UpdateStudentCommand : IRequest
{
    public string Id { get; set; }
    public string? ClassId { get; set; }
    public string? Name { get; set; }
    public string? StudentNumber { get; set; }
    public string? SchoolId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? Grade { get; set; }
    public string? Section { get; set; }
    public StudentStatus? Status { get; set; }
}