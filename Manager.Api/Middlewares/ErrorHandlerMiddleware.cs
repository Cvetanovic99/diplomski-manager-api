using Manager.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Api.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new ErrorResponse { Message = ex.Message, Code = 500 };

                switch (ex)
                {
                    case ApiException e:
                        response.StatusCode = e.Code;
                        responseModel.Code = e.Code;
                        responseModel.Message = e.Message;

                        break;
                    default:
                        response.StatusCode = 500;
                        break;
                }

                await response.WriteAsJsonAsync(responseModel);
            }
        }

        private class ErrorResponse
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }
    }
}
