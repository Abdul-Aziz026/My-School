using MediatR;

namespace Contracts.Commands
{
    public class NotifyStudentsCommand : IRequest<string>
    {
        public string Message { get; set; } = string.Empty;
    }
}
