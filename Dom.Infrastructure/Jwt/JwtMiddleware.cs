using Microsoft.AspNetCore.Http;

namespace Taxiverk.Infrastructure.Jwt
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context,  JwtService jwtService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var model = jwtService.ValidateToken(token);

            if (model != null)
            {
                if (model.Type == JwtUserType.User)
                {
                    context.Items["UserId"] = model.Id;
                }
                else
                {
                    context.Items["ClientId"] = model.Id;
                }
            }

            await _next(context);
        }
    }
}
