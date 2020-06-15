﻿using System;

 namespace MCMS.Exceptions
{
    public class KnownException : Exception
    {
        public int Code { get; }
        public string HtmlMessage { get; }

        public KnownException(string message, int code = 400, Exception innerException = null,
            string htmlMessage = null) : base(message, innerException)
        {
            Code = code;
            HtmlMessage = htmlMessage ?? message;
        }
    }
}