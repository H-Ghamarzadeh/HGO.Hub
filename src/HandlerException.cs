namespace HGO.Hub;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
[Obsolete]
public class HandlerException<T>
{
    /// <summary>
    /// 
    /// </summary>
    public Exception? Exception { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Type? HandlerType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public T? InputData { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int HandlerDefaultOrder { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int HandlerOrderInPipeline { get; set; }
}