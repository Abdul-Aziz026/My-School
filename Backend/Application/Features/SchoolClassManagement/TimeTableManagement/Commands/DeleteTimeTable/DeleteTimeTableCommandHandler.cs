
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Commands.DeleteTimeTable;

public class DeleteTimeTableCommandHandler : IRequestHandler<DeleteTimeTableCommand>
{
    private readonly ITimeTableRepository _timeTableRepository;
    public DeleteTimeTableCommandHandler(ITimeTableRepository timeTableRepository)
    {
        _timeTableRepository = timeTableRepository;
    }
    public async Task Handle(DeleteTimeTableCommand request, CancellationToken cancellationToken)
    {
        var timeTable = await _timeTableRepository.GetByIdAsync<TimeTable>(request.Id);
        if (timeTable == null)
        {
            throw new NotFoundException("TimeTable not found");
        }
        var deleted = await _timeTableRepository.DeleteByIdAsync<TimeTable>(timeTable.Id);
        if (!deleted)
        {
            throw new Exception("Failed to delete TimeTable");
        }
    }
}
