using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SMS_API;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SMSAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
    
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);

            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                object[] error = null;
                switch (ex)
                {
                    case ServiceException serviceExpecption:
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        error = serviceExpecption.Errors;
                        break;             

                    case Exception expecption:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        error = new[] { expecption +$" Error Meessge: {expecption.Message}"}; 
                        break;
                }
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    statusCode = context.Response.StatusCode,
                    error

                }));
            }
        }

    }
}
