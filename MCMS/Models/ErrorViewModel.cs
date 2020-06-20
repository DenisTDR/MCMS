using System;

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
            return StatusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "The requested resource or page was not found",
                405 => "Method Not Allowed",
                _ => ""
            };
        }
    }
}