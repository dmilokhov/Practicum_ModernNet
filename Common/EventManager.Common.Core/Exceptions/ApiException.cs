namespace EventManager.Common.Core.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        protected ApiException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
