
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Commands.DeleteTimeTable;

public class DeleteTimeTableCommand : IRequest
{
    public string Id { get; set; }
    public DeleteTimeTableCommand(string id)
    {
        Id = id;
    }
}
