using System.Net;
using Microsoft.Extensions.Options;
using ApiGatewayProcesarSms.Entities;

namespace ApiGatewayProcesarSms.Handlers
{
    public class IdentityHandler : DelegatingHandler
    {
        private readonly MicroservicesAuth _auth;
        public IdentityHandler(IOptionsMonitor<MicroservicesAuth> optionsMonitor) => _auth = optionsMonitor.CurrentValue;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                request.Headers.Add("Authorization-Mego", "Auth-Mego " + _auth.ws_identity);
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadGateway);
                response.ReasonPhrase = HttpStatusCode.BadGateway.ToString();
                return await Task.FromResult<HttpResponseMessage>(response);
            }
        }
    }
}
