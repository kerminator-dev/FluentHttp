namespace FluentHTTPToolkit.Core.Exceptions
{
    public class UnexpectedStatusCodeException : Exception
    {
        public UnexpectedStatusCodeException(string message) 
            : base(message)
        {

        }
    }
}
