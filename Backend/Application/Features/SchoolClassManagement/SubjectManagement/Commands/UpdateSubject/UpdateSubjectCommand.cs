using MediatR;

namespace Application.Features.SchoolClassManagement.SubjectManagement.Commands.UpdateSubject;

public class UpdateSubjectCommand : IRequest
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public int? Credits { get; set; }
    public bool IsActive { get; set; }
}