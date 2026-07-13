using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace MS_Updates.Extensions
{
     public static class MiddlewareExtension
     {
          public static void UseGlobalExceptionHandler(this IApplicationBuilder app, ILogger logger)
          {
               // error handling pipeline (middleware)
               app.UseExceptionHandler(exceptionHandlerApp =>
               {
                    exceptionHandlerApp.Run(async context =>
                    {
                         context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                         // using static System.Net.Mime.MediaTypeNames;
                         context.Response.ContentType = MediaTypeNames.Application.Json;
                         var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                         // handle custom exceptions...
                         // Check if it's a cosmos exception - key is not found
                         if (exceptionHandlerPathFeature?.Error is KeyNotFoundException keyNotFoundExp)
                         {
                              context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                         }

                         var baseException = exceptionHandlerPathFeature?.Error?.GetBaseException();

                         // logging (send error, stack track messages to Azure Application Insights)
                         logger.LogError(baseException?.Message);
                         logger.LogError(baseException?.StackTrace);

                         // return response
                         var errorResult = JsonSerializer.Serialize(new
                         {
                              error = baseException?.Message,
                              message = exceptionHandlerPathFeature?.Error?.Message
                         });

                         await context.Response.WriteAsync(errorResult);
                    });
               });
          }
     }
}
