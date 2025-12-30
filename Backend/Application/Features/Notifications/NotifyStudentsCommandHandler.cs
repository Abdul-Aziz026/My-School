using Application.Interfaces.Publisher;
using Application.Interfaces.Repositories;
using Contracts.Commands;
using Contracts.Events;
using Domain.Entities;
using MediatR;

namespace Application.Features.Notifications;

public class NotifyStudentsCommandHandler : IRequestHandler<NotifyStudentsCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageBus _messageBus;
    public NotifyStudentsCommandHandler(IUserRepository userRepository,
                                        IMessageBus messageBus)
    {
        _userRepository = userRepository;
        _messageBus = messageBus;
    }
    public async Task Handle(NotifyStudentsCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;
        var users = await _userRepository.GetAllAsync<User>();
        foreach (var user in users)
        {
            await _messageBus.PublishAsync(new StudentNotifiedEvent()
            {
                Name = user.UserName,
                Email = user.Email,
                Subject = "May Allah Guide & Bless You in 2026 🌙",
                Body = message
            });
        }
        return;// Unit.Value;

    }
}   
