using AuthDemo.API.Middleware;

namespace AuthDemo.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
