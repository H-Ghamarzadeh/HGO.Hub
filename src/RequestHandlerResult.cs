namespace HGO.Hub
{
    /// <summary>
    /// Response for a Request Handler
    /// </summary>
    /// <typeparam name="TRes">Request Handler Result Type</typeparam>
    public class RequestHandlerResult<TRes>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="handled"></param>
        public RequestHandlerResult(TRes result, bool handled = true)
        {
            Result = result;
            Handled = handled;
        }

        /// <summary>
        /// If set to true than execution of pipeline will be stopped and current value of "Result" property will be returned.
        /// </summary>
        public bool Handled { get; set; } 
        /// <summary>
        /// Result of current Request Handler
        /// </summary>
        public TRes Result { get; set; }
    }
}
