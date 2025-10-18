using Entities.ErrorModel;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Services.Abstracts;
using System.Net;

namespace BookShopWeb.Extensions
{
    public static class ExceptionMiddlewareExtensions // bu bir global hata yakalama middleware'i
    {
        public static void ConfigureExceptionHandler(this WebApplication app, ILoggerService logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    // bu satırdaki kod silindi
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null) // null değilse hata var demektir, null ise hata yok demektir
                    {
                        // burada ise revize edildi
                        context.Response.StatusCode = contextFeature.Error switch
                        {
                            NotFoundException => StatusCodes.Status404NotFound,
                            _ => StatusCodes.Status500InternalServerError
                        };

                        logger.LogError($"Something went wrong: {contextFeature.Error}"); // hata logluyoruz

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message // burası da revize
                        }.ToString());
                    }
                });
            });
        }
    }
}
