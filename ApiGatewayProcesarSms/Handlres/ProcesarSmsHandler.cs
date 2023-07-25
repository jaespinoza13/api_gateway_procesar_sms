using System.Net;
using Microsoft.Extensions.Options;
using ApiGatewayProcesarSms.Entities;

namespace ApiGatewayProcesarSms.Handlers
{
    public class ProcesarSmsHandler : DelegatingHandler
    {
        
        private readonly MicroservicesAuth _auth;

        public ProcesarSmsHandler(IOptionsMonitor<MicroservicesAuth> auth, IOptionsMonitor<ApiSettings> settings)
        {
            this._auth = auth.CurrentValue;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                // Enviar credenciales de autenticacion al servicio wsProcesarSms
                request.Headers.Add("Authorization-Mego", "Auth-Mego " + _auth.ws_procesar_sms);
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
