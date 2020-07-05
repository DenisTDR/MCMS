using System;

namespace MCMS.Base.Exceptions
{
    public class KnownException : Exception
    {
        public int Code { get; }
        public string HtmlMessage { get; }

        private readonly string _message;

        public override string Message => _message ?? ErrorMessageFromCode(Code) ?? base.Message;

        public KnownException(string message = null, int code = 400, Exception innerException = null,
            string htmlMessage = null) : base(message, innerException)
        {
            Code = code;
            HtmlMessage = htmlMessage ?? message;
            _message = message;
        }

        public static string ErrorMessageFromCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "The requested resource or page was not found",
                405 => "Method Not Allowed",
                _ => null
            };
        }
    }
}