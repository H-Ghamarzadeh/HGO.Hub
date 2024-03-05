namespace HGO.Hub.Interfaces.Requests;

/// <summary>
///  Marker interface to represent a request.
/// </summary>
/// <typeparam name="T">The type of response object, which will be returned by handler of this request</typeparam>
public interface IRequest<T> { }