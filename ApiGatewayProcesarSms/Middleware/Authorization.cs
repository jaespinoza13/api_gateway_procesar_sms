using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using ApiGatewayProcesarSms.Entities;

namespace ApiGatewayProcesarSms.Middleware
{
    public static class AuthorizationExtensions
    {
        public static IApplicationBuilder UseAuthorizationMego(this IApplicationBuilder app)
        {
            return app.UseMiddleware<Authorization>();
        }
    }
    public class Authorization
    {
        private readonly RequestDelegate _next;
        private readonly ApiSettings _settings;
        public Authorization(RequestDelegate next, IOptionsMonitor<ApiSettings> setttings)
        {
            this._next = next;
            this._settings = setttings.CurrentValue;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string authHeader = httpContext.Request.Headers["Authorization-Gateway"];

            try
            {
                if (authHeader != null && authHeader.StartsWith("Auth-Gateway"))
                {
                    string endodedAuthorization = authHeader.Substring("Auth-Gateway ".Length).Trim();
                    if (endodedAuthorization.Equals(_settings.auth_gateway_procesarsms))
                    {
                        await _next.Invoke(httpContext);
                    }
                    else
                    {
                        httpContext.Response.StatusCode = Convert.ToInt32(HttpStatusCode.Unauthorized);
                    }
                }
                else
                {
                    httpContext.Response.StatusCode = Convert.ToInt32(HttpStatusCode.Unauthorized);
                }
            }
            catch (WebException ex)
            {
                httpContext.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadGateway);
                byte[] data = Encoding.UTF8.GetBytes(ex.Message.ToString());
                await httpContext.Response.Body.WriteAsync(data, 0, data.Length);
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
