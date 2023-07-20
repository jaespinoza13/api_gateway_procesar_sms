using System.Text;
using System.Text.Json;
using ApiGatewayProcesarSms.Entities;

namespace ApiGatewayProcesarSms.Services
{
    public class TokenService
    {
        private readonly MicroservicesAuth _auth;
        private readonly ApiSettings _settings;

        public TokenService(MicroservicesAuth auth, ApiSettings settings)
        {
            _auth = auth;
            _settings = settings;
        }

        public async Task<string> GenerarTokenTemporal(string body)
        {
            Dictionary<string, object> respuesta = await PostRestServiceDataAsync<Dictionary<string, object>>(
                    body,
                    _settings.ServicioIdentity + "AUTENTICARSE_INVITADO_INT"!,
                    String.Empty,
                    _auth.ws_identity!
                );
            return respuesta["str_token"].ToString()!;
        }

        public async Task<T> PostRestServiceDataAsync<T>(string serializedData,
            string serviceAddress,
            string parameteres,
            string auth)
        {
            HttpClient client = new();
            client.BaseAddress = new Uri(serviceAddress);
            client.DefaultRequestHeaders.Add("Authorization-Mego", "Auth-Mego " + auth);
            client.Timeout = TimeSpan.FromSeconds(10);
            var httpContent = new StringContent(serializedData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(parameteres, httpContent);
            string resultadoJson = await response.Content.ReadAsStringAsync();

            T respuesta = default(T)!;
            if (response.IsSuccessStatusCode)
            {
                respuesta = JsonSerializer.Deserialize<T>(resultadoJson)!;
            }
            return respuesta;
        }
    }
}
