using APIs_Signature_DigiGO.Iservices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;

namespace APIs_Signature_DigiGO.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TokenService> _logger;
        private string? _cachedToken;
        private DateTime _tokenExpiryTime;
        private readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1, 1);

        // Idéalement, ces valeurs devraient provenir de votre configuration (appsettings.json)
        private const string TokenUrl = "https://doc-sign-api.stb.com.tn/api/user/request_token";
        private const string ApiKey = "73526BAF092A54893D9E57614BE32444";
        private const string Username = "admin_stb@stbbank.com.tn";
        private const string Password = "F83n`EcjK£(q£qI4N";

        public TokenService(IHttpClientFactory httpClientFactory, ILogger<TokenService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _tokenExpiryTime = DateTime.UtcNow;
        }

        public async Task<string> GetApiTokenAsync()
        {
            // Fast path
            if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiryTime > DateTime.UtcNow)
            {
                _logger.LogDebug("Returning cached token, expires at {ExpiryUtc}", _tokenExpiryTime);
                return _cachedToken;
            }

            _logger.LogDebug("Token expired or missing, attempting to acquire token lock to refresh.");
            await _tokenLock.WaitAsync();
            try
            {
                // Double-check after acquiring lock
                if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiryTime > DateTime.UtcNow)
                {
                    _logger.LogDebug("Another thread refreshed the token while waiting for lock. Returning cached token, expires at {ExpiryUtc}", _tokenExpiryTime);
                    return _cachedToken;
                }

                _logger.LogInformation("Requesting new API token from {TokenUrl}. Note: move credentials to configuration.", TokenUrl);

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Remove("X-Api-Key");
                client.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);

                var requestBody = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", Username),
                    new KeyValuePair<string, string>("password", Password)
                });

                HttpResponseMessage response;
                try
                {
                    response = await client.PostAsync(TokenUrl, requestBody);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "HTTP request to token endpoint failed.");
                    throw;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Token endpoint returned StatusCode={StatusCode} Body={ResponseBody}", response.StatusCode, Truncate(responseContent, 1000));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Token endpoint returned non-success status {StatusCode}", response.StatusCode);
                    response.EnsureSuccessStatusCode();
                }

                // DTO simple pour désérialiser la réponse du token
                TokenResponseDto? tokenResponse = null;
                try
                {
                    tokenResponse = JsonSerializer.Deserialize<TokenResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize token response.");
                    throw;
                }

                if (tokenResponse?.Data?.Token != null)
                {
                    _cachedToken = tokenResponse.Data.Token;
                    // Supposons que le jeton est valide pour 1 heure (ajustez si nécessaire)
                    _tokenExpiryTime = DateTime.UtcNow.AddHours(1);
                    _logger.LogInformation("New token acquired, will expire at {ExpiryUtc}", _tokenExpiryTime);
                    return _cachedToken;
                }

                _logger.LogError("Token response did not contain a token. Full response: {ResponseBody}", Truncate(responseContent, 2000));
                throw new InvalidOperationException("Impossible de récupérer le jeton d'API.");
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        private static string Truncate(string? input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.Length <= maxLength ? input : input.Substring(0, maxLength) + "...(truncated)";
        }
    }

    // DTO pour la réponse du token
    public class TokenDataDto { public string? Token { get; set; } }
    public class TokenResponseDto { public TokenDataDto? Data { get; set; } }
}
