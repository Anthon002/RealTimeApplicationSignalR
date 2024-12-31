

namespace RealTimeApplication.MVC.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string LoginUrl = "Identity/Login";
        private readonly string SignUpUrl = "Identity/SignUp";
        public AuthenticationMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Invoke(HttpContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var name = httpContext?.User.Identity?.Name;
            var identity = httpContext?.User.Identity;

            var url = context.Request.Path.Value ?? "/";
            if (url.Contains(LoginUrl) || url.Contains(SignUpUrl))
            {
                await _next(context);
                return;
            }
            if (name is not null)
            {
                context.Response.Redirect(LoginUrl);
            }
        }
    }
    public static class UseAuthenticationMiddlewareClass
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}