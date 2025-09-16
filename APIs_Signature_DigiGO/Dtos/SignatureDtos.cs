using System.Text.Json.Serialization;

namespace APIs_Signature_DigiGO.Dtos
{
    // Request Body
    public class SignatoryDto
    {
        public string? Mail { get; set; }
        public string? FullName { get; set; }
        public string? Company { get; set; }
        public string? Department { get; set; }
        public string? Type { get; set; }
    }

    public class RappelDto
    {
        public string? Frequency { get; set; }
        public string? ReminderTime { get; set; }
    }

    public class SettingsDto
    {
        public string? Order { get; set; }
        public RappelDto? Rappel { get; set; }
    }

    public class SignatureRequestDto
    {
        public string? IdDemand { get; set; }
        public string? Service { get; set; }
        public string? FileName { get; set; }
        public string? Document { get; set; } // Base64 encoded string
        public List<SignatoryDto>? Signatories { get; set; }
        public SettingsDto? Settings { get; set; }
    }

    // Response Body
    public class SignatureResponseDto
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("responseCode")]
        public string? ResponseCode { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}