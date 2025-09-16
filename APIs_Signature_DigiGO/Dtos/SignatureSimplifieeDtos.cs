using System.ComponentModel.DataAnnotations;

namespace APIs_Signature_DigiGO.Dtos
{
    public class SignatureSimplifieeRequestDto
    {
        [Required]
        public string? IdDigital { get; set; }

        [Required]
        public string? IdDemand { get; set; }

        [Required]
        public string? Service { get; set; }

        [Required]
        public string? FileName { get; set; }

        [Required]
        public string? Document { get; set; } // Contenu du document en Base64

        [Required]
        public string? Matricule { get; set; }
    }
}