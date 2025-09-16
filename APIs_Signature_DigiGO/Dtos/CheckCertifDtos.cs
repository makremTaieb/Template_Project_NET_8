using System.Text.Json.Serialization;

namespace APIs_Signature_DigiGO.Dtos
{
    public class CertifInfos
    {
        public string? Email { get; set; }
        public string? OwnerName { get; set; }
        public string? SerialNumber { get; set; }
        public string? Issuer { get; set; }
        public string? Country { get; set; }
        public string? ValidFrom { get; set; }
        public string? ValidTo { get; set; }
    }

    public class CheckCertifResponseDto
    {
        [JsonPropertyName("statut")]
        public string? Statut { get; set; }

        [JsonPropertyName("responseCode")]
        public string? ResponseCode { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("infos")]
        public CertifInfos? Infos { get; set; }
    }
}