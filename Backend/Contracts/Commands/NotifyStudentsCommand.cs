using MediatR;

namespace Contracts.Commands
{
    public class NotifyStudentsCommand : IRequest
    {
        public string Message { get; set; }
    }
}
