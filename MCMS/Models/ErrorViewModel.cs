using System;
using MCMS.Base.Exceptions;

namespace MCMS.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public Exception Exception { get; set; }

        public int StatusCode { get; set; }

        public string StatusCodeMessage()
        {
            return KnownException.ErrorMessageFromCode(StatusCode);
        }
    }
}