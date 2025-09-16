using System.Text.Json.Serialization;

namespace APIs_Signature_DigiGO.Dtos
{
    public class DemandInfo
    {
        [JsonPropertyName("statut")]
        public string? Statut { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("file")]
        public string? File { get; set; }

        [JsonPropertyName("date_signature")]
        public string? DateSignature { get; set; }
    }

    public class CheckDemandResponseDto
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("responseCode")]
        public string? ResponseCode { get; set; }

        [JsonPropertyName("info")]
        public List<DemandInfo>? Info { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}