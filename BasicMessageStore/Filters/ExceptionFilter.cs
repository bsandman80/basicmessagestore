﻿using System;
using BasicMessageStore.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BasicMessageStore.Filters
{
    /// <summary>
    /// Filter to catch unhandled exceptions. 
    /// TODO: Should implement error logging
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExceptionFilter : Attribute, IExceptionFilter
    {        
        public void OnException(ExceptionContext context)
        {
            // This is currently implemented as a synchronous filter
            // but with added logging the async variant is motivated
            if (context.Exception is MessageStoreException mse)
            {
                // Translate applicable application error codes into http responses
                switch (mse.ErrorCode)
                {
                    case ErrorCodes.ResourceNotFound:
                        context.Result = new NotFoundObjectResult(mse.Message);
                        break;
                    case ErrorCodes.NotAuthorized:
                        context.Result = new UnauthorizedResult();
                        break;
                    case ErrorCodes.Required:
                    case ErrorCodes.Unique:
                        context.Result = new BadRequestObjectResult(mse.Message);
                        break;
                }
            }
        }
    }
}