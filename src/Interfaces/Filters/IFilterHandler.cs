namespace HGO.Hub.Interfaces.Filters
{
    /// <summary>
    /// Defines a handler for a data filter.
    /// </summary>
    /// <typeparam name="T">The type of data being handled</typeparam>
    public interface IFilterHandler<T>
    {
        /// <summary>
        /// Handles a data filtering request.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>Filtered data</returns>
        Task<T> Handle(T data);

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
