
# HGO.Hub (.Net Library for implementing In-Process Messaging mechanism)
A simple library for implement Event Sourcing, Pub/Sub, Mediator, CQRS Pattern with multiple handlers in .NET, with this package you can easily implement Wordpress Hooks (Action/Filter) in your ASP.Net project.

[![NuGet version (HGO.Hub)](https://img.shields.io/nuget/v/HGO.Hub)](https://www.nuget.org/packages/HGO.Hub/)

### Installing HGO.Hub
You should install  [HGO.Hub with NuGet](https://www.nuget.org/packages/HGO.Hub):

```
Install-Package HGO.Hub
```

Or via the .NET Core command line interface:

```cs
dotnet add package HGO.Hub
```

Either commands, from Package Manager Console or .NET Core CLI, will download and install HGO.Hub and all required dependencies.

### Registering with  `IServiceCollection`

HGO.Hub supports  `Microsoft.Extensions.DependencyInjection.Abstractions`  directly. To register various HGO.Hub services and handlers:

```cs
services.AddHgoHub(configuration =>
{
    configuration.HandlersDefaultLifetime = ServiceLifetime.Scoped;
    configuration.RegisterServicesFromAssemblyContaining<Startup>();
});
```
or with an assembly:
```cs
services.AddHgoHub(configuration =>
{
    configuration.HandlersDefaultLifetime = ServiceLifetime.Scoped;
    configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});
```
This registers:

-   `IHub`  as Singleton
-   `IEventHandler<>`  as Scoped
-   `IActionHandler<>`  as Scoped
-   `IFilterHandler<>`  as Scoped
-   `IRequestHandler<,>`  as Scoped

### Messages Type
There is 4 type of messages which you can send via `IHub` into pipeline, each type of messages will be delivered automatically to corresponding handler.

 - `Event`: This type of messages will be published into pipeline as an event, events will be handled by multiple corresponding handlers asynchronously and in parallel. Also, events do not return any value.
 - `Action`: This type of messages will be published into pipeline as an action, actions will be handled by multiple corresponding handlers asynchronously and sequentially (based on `Order` property - lower numbers correspond with earlier execution). These types of messages are equal to events, with the difference that they are executed sequentially. Also, actions do not return any value. In general, actions are similar to WordPress Actions.
 - `Filter`: Asynchronously and sequentially (based on `Order` property - lower numbers correspond with earlier execution) applies all the corresponding filter handlers which registered in the pipeline to the data and returns the filtered data. Samething as Wordpress Filters.
 - `Request`: Asynchronously will sent a request (Command/Query) to corresponding handler and will return the response. You can have multiple handler for a request, but just one of them will be executed (which has larger number in `Priority` property). with type of messages you can implement CQRS and Mediator pattern.

### Publish an Event example:

 1. First add HGO.Hub service:
```cs
services.AddHgoHub(configuration =>
{
    configuration.HandlersDefaultLifetime = ServiceLifetime.Scoped;
    configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});
```
2. Define a class which inherits from IEvent interface and add your desired properties which contain information about event:
```cs
public class OnUserRegistered: IEvent
{
    public int RegisteredUserId { get; set; }
}
```
3. Now you must define a handler for this event:
```cs
public class SendEmailOnUserRegisteredEventHandler(IHub hub, IEmailService emailService) : IEventHandler<OnUserRegistered>
{
    public async Task Handle(OnUserRegistered @event)
    {
        var user = await hub.RequestAsync(new GetUserRequest(@event.RegisteredUserId));
        await emailService.SendEmail(user.EmailAddress, "Hello", $"Dear {user.FirstName} {user.LastName}, Thank you for join us!");
    }
}
```
4. Now you can publish the `OnUserRegistered` event when a user is registered in the system, you can publish the event via `Hub` class, the `Hub` class will identify all handlers for `OnUserRegistered` event and will sent the event for all of them in parallel and asynchronously :
```cs
await _hub.PublishEventAsync(new OnUserRegistered() { RegisteredUserId = userId });
```

### More examples:
For more examples please check `HGO.Hub.Test` project, I write some unit tests for all type of messages!

#### If you have any kind of question or problem, I will be happy to contact me via email: h.ghamarzadeh@hotmail.com
