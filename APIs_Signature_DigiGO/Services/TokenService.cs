using APIs_Signature_DigiGO.Iservices;
using System.Text.Json;

namespace APIs_Signature_DigiGO.Services
{

        public class TokenService : ITokenService
        {
            private readonly IHttpClientFactory _httpClientFactory;
            private string? _cachedToken;
            private DateTime _tokenExpiryTime;

            // Idéalement, ces valeurs devraient provenir de votre configuration (appsettings.json )
            private const string TokenUrl = "https://doc-sign-api.stb.com.tn/api/user/request_token";
            private const string ApiKey = "73526BAF092A54893D9E57614BE32444";
            private const string Username = "admin_stb@stbbank.com.tn";
            private const string Password = "F83n`EcjK£(q£qI4N";

            public TokenService(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
                _tokenExpiryTime = DateTime.UtcNow;
            }

            public async Task<string> GetApiTokenAsync()
            {
                if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiryTime > DateTime.UtcNow)
                {
                    return _cachedToken;
                }

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);

                var requestBody = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("username", Username),
                new KeyValuePair<string, string>("password", Password)
            });

                var response = await client.PostAsync(TokenUrl, requestBody);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                // DTO simple pour désérialiser la réponse du token
                var tokenResponse = JsonSerializer.Deserialize<TokenResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tokenResponse?.Data?.Token != null)
                {
                    _cachedToken = tokenResponse.Data.Token;
                    // Supposons que le jeton est valide pour 1 heure (ajustez si nécessaire)
                    _tokenExpiryTime = DateTime.UtcNow.AddHours(1);
                    return _cachedToken;
                }

                throw new InvalidOperationException("Impossible de récupérer le jeton d'API.");
            }
        }

        // DTO pour la réponse du token
        public class TokenDataDto { public string? Token { get; set; } }
        public class TokenResponseDto { public TokenDataDto? Data { get; set; } }
    
}
