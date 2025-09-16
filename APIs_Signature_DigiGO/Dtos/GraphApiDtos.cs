using System.Text.Json.Serialization;

namespace APIs_Signature_DigiGO.Dtos
{
    // Pour la réponse du jeton Azure AD
    public class AzureTokenResponseDto
    {
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }

    // Pour la réponse de l'utilisateur de Microsoft Graph
    public class GraphUserDto
    {
        public string? DisplayName { get; set; }
        public string? Mail { get; set; }
        public string? UserPrincipalName { get; set; }
        public string? Id { get; set; }
    }

    public class GraphUserResponseDto
    {
        [JsonPropertyName("@odata.count")]
        public int Count { get; set; }
        public List<GraphUserDto>? Value { get; set; }
    }
}