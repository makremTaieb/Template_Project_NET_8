using APIs_Signature_DigiGO.Dtos;

namespace APIs_Signature_DigiGO.Iservices
{
    public interface ILegalisationService
    {
        Task<CheckDemandResponseDto> CheckDemandAsync(string demandeId);
        Task<CheckCertifResponseDto> CheckCertifAsync(string email);
        Task<VerifResponseDto> VerifSignatureAsync(string idDemand);
        Task<SignatureResponseDto> CreateSignatureDemandAsync(SignatureRequestDto request);
        Task<SignatureResponseDto> CreateSignatureSimplifieeAsync(SignatureSimplifieeRequestDto request);

    }
}
