using APIs_Signature_DigiGO.Dtos;
using APIs_Signature_DigiGO.Iservices;
using System.Net.Http.Headers;
using System.Text.Json;

namespace APIs_Signature_DigiGO.Services
{
    public class GraphApiService : IGraphApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private string? _cachedToken;
        private DateTime _tokenExpiryTime;

        public GraphApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _tokenExpiryTime = DateTime.UtcNow;
        }

        private async Task<string> GetGraphApiTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiryTime > DateTime.UtcNow)
            {
                return _cachedToken;
            }

            var azureAdConfig = _configuration.GetSection("AzureAd");
            var tokenUrl = $"{azureAdConfig["Instance"]}{azureAdConfig["TenantId"]}/oauth2/v2.0/token";

            var client = _httpClientFactory.CreateClient();
            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", azureAdConfig["ClientId"]!),
                new KeyValuePair<string, string>("scope", azureAdConfig["Scope"]!),
                new KeyValuePair<string, string>("client_secret", azureAdConfig["ClientSecret"]!),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await client.PostAsync(tokenUrl, requestBody);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<AzureTokenResponseDto>(responseContent);

            if (string.IsNullOrEmpty(tokenResponse?.AccessToken))
            {
                throw new InvalidOperationException("Impossible de récupérer le jeton d'accès pour Microsoft Graph.");
            }

            _cachedToken = tokenResponse.AccessToken;
            _tokenExpiryTime = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // Marge de sécurité de 60s

            return _cachedToken;
        }

        public async Task<(string? Email, string? FullName)> GetUserDataByMatriculeAsync(string matricule)
        {
            var token = await GetGraphApiTokenAsync();
            var graphApiConfig = _configuration.GetSection("GraphApi");

            var filter = string.Format(graphApiConfig["UserFilter"]!, matricule);
            var requestUrl = $"{graphApiConfig["BaseUrl"]}/{filter}";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var userResponse = JsonSerializer.Deserialize<GraphUserResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (userResponse?.Count > 0 && userResponse.Value?.FirstOrDefault() != null)
            {
                var user = userResponse.Value.First();
                return (user.Mail ?? user.UserPrincipalName, user.DisplayName);
            }

            throw new KeyNotFoundException($"Aucun utilisateur trouvé pour le matricule : {matricule}");
        }
    }
}