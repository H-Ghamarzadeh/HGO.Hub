namespace HGO.Hub.Interfaces.Actions
{
    /// <summary>
    /// Defines a handler for an action.
    /// </summary>
    /// <typeparam name="T">Type of action</typeparam>
    public interface IActionHandler<in T> where T : IAction
    {
        /// <summary>
        /// Handles an action.
        /// </summary>
        /// <param name="action">Information about published action</param>
        /// <returns></returns>
        Task Handle(T action);

        /// <summary>
        /// Order of handler in pipeline. Lower numbers will be executed earlier.
        /// Default value is 0
        /// </summary>
        int Order { get; }

        /// <summary>
        /// If the value of this property is TRUE, subsequent handlers in the pipeline will not be executed.
        /// </summary>
        bool Stop { get; }
    }
}
