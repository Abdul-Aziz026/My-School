namespace Contracts.Commands;

using MediatR;

public class NotifyStudentsCommand : IRequest<Unit>
{
    public string Message { get; set; }
}
