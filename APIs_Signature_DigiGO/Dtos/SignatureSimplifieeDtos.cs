using System.ComponentModel.DataAnnotations;

namespace APIs_Signature_DigiGO.Dtos
{
public enum SignatureSendType
{
    Sequential,
    Parallel
}

    public class SignatureSimplifieeRequestDto
    {


        [Required]
        public string? IdDigital { get; set; }

        [Required]
        public string? ClientType { get; set; }

        [Required]
        public string? RequestId  { get; set; }

        [Required]
        public string? Service { get; set; }

        [Required]
        public string? FileName { get; set; }

        [Required]
        public string? Document { get; set; } // Contenu du document en Base64

        [Required]
        public List<string>? Signatories { get; set; } // Liste des matricules

        [Required]
    public SignatureSendType SendType { get; set; } // Sequential or Parallel

    }
}