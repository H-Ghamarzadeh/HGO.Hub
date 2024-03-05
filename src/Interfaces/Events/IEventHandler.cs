namespace HGO.Hub.Interfaces.Events
{
    /// <summary>
    /// Defines a handler for an event.
    /// </summary>
    /// <typeparam name="T">Type of event</typeparam>
    public interface IEventHandler<in T> where T : IEvent
    {
        /// <summary>
        /// Handles an event.
        /// </summary>
        /// <param name="event">Information about published event</param>
        /// <returns></returns>
        Task Handle(T @event);
    }
}
