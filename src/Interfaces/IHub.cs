using HGO.Hub.Interfaces.Actions;
using HGO.Hub.Interfaces.Events;
using HGO.Hub.Interfaces.Requests;

namespace HGO.Hub.Interfaces
{
    /// <summary>
    /// Publish a data filtering request or event/action or request to the pipeline to be handled by multiple handlers.
    /// </summary>
    public interface IHub
    {
        /// <summary>
        /// Publish an event to multiple corresponding handlers asynchronously and in parallel
        /// </summary>
        /// <param name="event">An object which inherits from the IEvent interface and contains the event information</param>
        /// <param name="handleExceptions">If set to true, all exceptions will be caught</param>
        /// <typeparam name="T">Type of Event</typeparam>
        /// <returns></returns>
        Task PublishEventAsync<T>(T @event, bool handleExceptions = false) where T : IEvent;

        /// <summary>
        /// Send a Do_Action request to multiple corresponding handlers asynchronously and sequentially
        /// </summary>
        /// <param name="action">An object which inherits from the IAction interface and contains the action information</param>
        /// <param name="handleExceptions">If set to true, all exceptions will be caught</param>
        /// <typeparam name="T">Type of Action</typeparam>
        /// <returns></returns>
        Task DoActionAsync<T>(T action, bool handleExceptions = false) where T : IAction;

        /// <summary>
        /// Send a Do_Action request to multiple corresponding handlers asynchronously and sequentially
        /// and handle all exceptions which may occur in handlers
        /// </summary>
        /// <param name="action">An object which inherits from the IAction interface and contains the action information</param>
        /// <typeparam name="T">Type of Action</typeparam>
        /// <returns>List of occured exceptions</returns>
        [Obsolete("DoActionAndHandleExceptionsAsync is deprecated, please use DoActionAsync instead.")]
        Task<List<HandlerException<T>>> DoActionAndHandleExceptionsAsync<T>(T action) where T : IAction;

        /// <summary>
        /// Asynchronously and sequentially applies all the corresponding filter handlers which registered
        /// in the pipeline to the data and returns the filtered data
        /// </summary>
        /// <param name="data">The object which filters will be applied on it</param>
        /// <param name="handleExceptions">If set to true, all exceptions will be caught</param>
        /// <typeparam name="T">Type of data object</typeparam>
        /// <returns>Filtered data</returns>
        Task<T> ApplyFiltersAsync<T>(T data, bool handleExceptions = false);

        /// <summary>
        /// Asynchronously and sequentially applies all the corresponding filter handlers which registered
        /// in the pipeline to the data and handle all exceptions which may occur in handlers and returns the filtered data
        /// </summary>
        /// <param name="data">The object which filters will be applied on it</param>
        /// <typeparam name="T">Type of data object</typeparam>
        /// <returns>Filtered data</returns>
        [Obsolete("ApplyFiltersAndHandleExceptionsAsync is deprecated, please use ApplyFiltersAsync instead.")]
        Task<FilterHandlerResult<T>> ApplyFiltersAndHandleExceptionsAsync<T>(T data);

        /// <summary>
        /// Asynchronously send a request (Command/Query) to corresponding handler and return the response
        /// </summary>
        /// <param name="request">An object which inherits from the IRequest interface and contains the request information</param>
        /// <param name="autoApplyFiltersOnResponse">If true, then all corresponding filter handlers will be applied on response object</param>
        /// <param name="catchExceptionsAndRunNextHandler">If set to true, all exceptions will be caught and an attempt will be made to retrieve data from the next request handler in the pipeline.</param>
        /// <typeparam name="TRes">Type of response object</typeparam>
        /// <returns>Generated response</returns>
        Task<TRes> RequestAsync<TRes>(IRequest<TRes> request, bool autoApplyFiltersOnResponse = true, bool catchExceptionsAndRunNextHandler = false);
    }
}
