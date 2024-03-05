using HGO.Hub.Interfaces;
using HGO.Hub.Interfaces.Events;
using HGO.Hub.Test.CQRS.Queries;
using HGO.Hub.Test.Services;

namespace HGO.Hub.Test.Events
{
    public class OnUserRegistered: IEvent
    {
        public int RegisteredUserId { get; set; }
    }

    public class SendEmailOnUserRegisteredEventHandler(IHub hub, IEmailService emailService) : IEventHandler<OnUserRegistered>
    {
        public async Task Handle(OnUserRegistered @event)
        {
            var user = await hub.RequestAsync(new GetUserRequest(@event.RegisteredUserId));
            await emailService.SendEmail(user.EmailAddress, "Hello", $"Dear {user.FirstName} {user.LastName}, Thank you for join us!");
        }
    }

    public class SendSmsOnUserRegisteredEventHandler(IHub hub, ISmsService smsService) : IEventHandler<OnUserRegistered>
    {
        public async Task Handle(OnUserRegistered @event)
        {
            var user = await hub.RequestAsync(new GetUserRequest(@event.RegisteredUserId));
            await smsService.SendSms(user.PhoneNumber, $"Dear {user.FirstName} {user.LastName}, Thank you for join us!");
        }
    }
}
