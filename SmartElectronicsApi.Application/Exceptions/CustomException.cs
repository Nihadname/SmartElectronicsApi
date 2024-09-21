using System;
using System.Collections.Generic;

namespace SmartElectronicsApi.Application.Exceptions
{
    public class CustomException : Exception
    {
        public int Code { get; set; }
        public Dictionary<string, string> Errors { get; set; } = new();

        // Constructor for a general exception with a status code and message
        public CustomException(int code, string message) : base(message)
        {
            Code = code;
        }

        // Constructor for adding a single error with key and message
        public CustomException(string errorKey, string errorMessage)
        {
            Errors.Add(errorKey, errorMessage);
        }

        // Constructor for adding multiple errors with a status code
        public CustomException(int code, Dictionary<string, string> errors, string message = null)
        {
            Code = code;
            Errors = errors;
            if (!string.IsNullOrEmpty(message))
            {
                base.HelpLink = message;
            }
        }

        // Constructor for adding a single error with key, message, and status code
        public CustomException(int code, string errorKey, string errorMessage, string message = null) : base(message)
        {
            Code = code;
            Errors.Add(errorKey, errorMessage);
        }
    }
}