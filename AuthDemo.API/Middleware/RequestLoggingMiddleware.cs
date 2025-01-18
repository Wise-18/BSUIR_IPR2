namespace AuthDemo.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                if (context.Response.StatusCode < 200 || context.Response.StatusCode > 299)
                {
                    var user = context.User.Identity?.Name ?? "anonymous";
                    _logger.LogWarning(
                        "Request {Method} {Path} by {User} responded with {StatusCode}",
                        context.Request.Method,
                        context.Request.Path,
                        user,
                        context.Response.StatusCode);
                }
            }
        }
    }
}
