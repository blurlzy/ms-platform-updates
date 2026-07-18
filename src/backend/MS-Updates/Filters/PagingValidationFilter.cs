namespace MS_Updates.Filters
{
     public sealed class PagingValidationFilter : IEndpointFilter
     {
          private const int MaxPageSize = 20;

          public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
          {
               var query = context.HttpContext.Request.Query;
               var pageIndex = GetQueryValue(query, "pageIndex");
               var pageSize = GetQueryValue(query, "pageSize");
               
               // validate page index
               if (pageIndex < 0)
               {
                    return ValueTask.FromResult<object?>(Results.BadRequest(new { error = "pageIndex must be greater than or equal to 0." }));
               }

               // validate page size (min is 10)
               if (pageSize > MaxPageSize || pageSize < 10)
               {
                    return ValueTask.FromResult<object?>(Results.BadRequest(new { error = $"pageSize must be greater or equal to 10 and less than or equal to {MaxPageSize}." }));
               }

               return next(context);
          }

          private static int? GetQueryValue(IQueryCollection query, string key)
          {
               return query.TryGetValue(key, out var values) && int.TryParse(values, out var value)
                    ? value
                    : null;
          }
     }
}
