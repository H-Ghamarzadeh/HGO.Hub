﻿namespace HGO.Hub;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
[Obsolete]
public class FilterHandlerResult<T>
{
    /// <summary>
    /// 
    /// </summary>
    public List<HandlerException<T>> Exceptions { get; set; } = [];
    /// <summary>
    /// 
    /// </summary>
    public T? Result { get; set; }
}