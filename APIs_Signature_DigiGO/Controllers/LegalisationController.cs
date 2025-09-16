using APIs_Signature_DigiGO.Dtos;
using APIs_Signature_DigiGO.Iservices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIs_Signature_DigiGO.Controllers
{
    [ApiController]
    [Route("api/legalisation")]
    public class LegalisationController : ControllerBase
    {
        private readonly ILegalisationService _legalisationService;

        public LegalisationController(ILegalisationService legalisationService)
        {
            _legalisationService = legalisationService;
        }

        [HttpGet("check-demand")]
        public async Task<IActionResult> CheckDemand([FromHeader(Name = "demandeId")] string demandeId)
        {
            if (string.IsNullOrEmpty(demandeId))
            {
                return BadRequest("Le header 'demandeId' est requis.");
            }
            var result = await _legalisationService.CheckDemandAsync(demandeId);
            return Ok(result);
        }

        [HttpGet("check-certif")]
        public async Task<IActionResult> CheckCertif([FromHeader(Name = "email")] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Le header 'email' est requis.");
            }
            var result = await _legalisationService.CheckCertifAsync(email);
            return Ok(result);
        }

        [HttpGet("verif-signature")]
        public async Task<IActionResult> VerifSignature([FromHeader(Name = "IdDemand")] string idDemand)
        {
            if (string.IsNullOrEmpty(idDemand))
            {
                return BadRequest("Le header 'IdDemand' est requis.");
            }
            var result = await _legalisationService.VerifSignatureAsync(idDemand);
            return Ok(result);
        }

        [HttpPost("signature-demande")]
        public async Task<IActionResult> CreateSignatureDemand([FromBody] SignatureRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _legalisationService.CreateSignatureDemandAsync(request);
            return Ok(result);
        }

        // NOUVEAU POINT DE TERMINAISON
        [HttpPost("signature")]
        public async Task<IActionResult> CreateSignatureSimplifiee([FromBody] SignatureSimplifieeRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _legalisationService.CreateSignatureSimplifieeAsync(request);

            // Si le service retourne un code d'erreur spécifique, on peut retourner un statut HTTP approprié
            if (result.ResponseCode == "AUTH-01")
            {
                return Unauthorized(result); // 401 Unauthorized
            }
            if (result.ResponseCode == "USER-02")
            {
                return NotFound(result); // 404 Not Found
            }

            return Ok(result);
        }
    }
}