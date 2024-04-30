namespace HGO.Hub.Interfaces.Requests;

/// <summary>
/// Defines a handler for a request.
/// </summary>
/// <typeparam name="TReq">The type of Request</typeparam>
/// <typeparam name="TRes">The type of Response</typeparam>
public interface IRequestHandler<in TReq, TRes> where TReq : IRequest<TRes>
{
    /// <summary>
    /// Handles a request and generate response.
    /// </summary>
    /// <param name="request">An object of IRequest interface which contain information about request</param>
    /// <returns>Response</returns>
    Task<RequestHandlerResult<TRes>> Handle(TReq request);

    /// <summary>
    /// Priority of handler. The handler with the largest value will ONLY be executed.
    /// Default value is 0
    /// </summary>
    int Priority { get; }
}