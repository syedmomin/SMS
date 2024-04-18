using System;
using System.Net;

namespace SMS_API
{
    public class ServiceException : Exception
    {
        public string[] Errors { get; }

        public ServiceException(params string[] errors)
        {
            this.Errors = errors;
        }
    }
}
