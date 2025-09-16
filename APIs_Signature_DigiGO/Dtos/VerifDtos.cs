using System.Text.Json.Serialization;

namespace APIs_Signature_DigiGO.Dtos
{
    public class VerifResponseDto
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("responseCode")]
        public string? ResponseCode { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("signatureComplete")]
        public bool? SignatureComplete { get; set; }
    }
}