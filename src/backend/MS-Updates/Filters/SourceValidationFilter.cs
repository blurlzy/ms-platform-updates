namespace MS_Updates.Filters
{
     public sealed class SourceValidationFilter : IEndpointFilter
     {
          private static readonly HashSet<string> AllowedSources =
          [
               "Azure",
               "Microsoft Foundry",
               "Microsoft Fabric",
               "GitHub",
               "Microsoft Copilot 365"
          ];

          public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
          {
               var source = context.HttpContext.Request.Query["source"].ToString();

               // source is optional
               if (string.IsNullOrWhiteSpace(source))
               {
                    return await next(context);
               }

               // ensure the source is valid (case-insensitive)
               var isValid = AllowedSources.Contains(source,StringComparer.OrdinalIgnoreCase);

               if (!isValid)
               {
                    return Results.BadRequest(new
                    {
                         error = $"Invalid source '{source}'.",
                         // allowedSources = AllowedSources
                    });
               }

               return await next(context);
          }
     }
}
