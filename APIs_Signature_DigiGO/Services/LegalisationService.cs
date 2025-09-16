using APIs_Signature_DigiGO.Dtos;
using APIs_Signature_DigiGO.Iservices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace APIs_Signature_DigiGO.Services
{
    public class LegalisationService : ILegalisationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LegalisationService> _logger;
        private readonly IGraphApiService _graphApiService; // Injection du nouveau service
        private const string BaseUrl = "https://legalisation.stb.com.tn/Signer/signature_document";

        public LegalisationService(
                IHttpClientFactory httpClientFactory,
                ITokenService tokenService,
                IGraphApiService graphApiService,
                ILogger<LegalisationService> logger) // Mise à jour du constructeur
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _graphApiService = graphApiService;
            _logger = logger;
        }

        private async Task<HttpClient> CreateClientWithTokenAsync()
        {
            _logger.LogDebug("Creating HTTP client and retrieving API token.");
            var client = _httpClientFactory.CreateClient();
            var token = await _tokenService.GetApiTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("TokenService returned empty token.");
            }
            client.DefaultRequestHeaders.Remove("token");
            client.DefaultRequestHeaders.Add("token", token);
            _logger.LogDebug("HTTP client created with token header.");
            return client;
        }

        public async Task<CheckDemandResponseDto> CheckDemandAsync(string demandeId)
        {
            _logger.LogInformation("CheckDemandAsync called for demandeId={DemandeId}", demandeId);
            try
            {
                var client = await CreateClientWithTokenAsync();
                client.DefaultRequestHeaders.Remove("demandeId");
                client.DefaultRequestHeaders.Add("demandeId", demandeId);

                var url = $"{BaseUrl}/check_demand";
                _logger.LogDebug("Sending GET request to {Url} with demandeId={DemandeId}", url, demandeId);

                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Received response StatusCode={StatusCode} Body={ResponseBody}", response.StatusCode, content);

                response.EnsureSuccessStatusCode();

                var dto = JsonSerializer.Deserialize<CheckDemandResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
                _logger.LogInformation("CheckDemandAsync completed for demandeId={DemandeId} with status={Status}", demandeId, dto?.Status);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckDemandAsync for demandeId={DemandeId}", demandeId);
                throw;
            }
        }

        public async Task<CheckCertifResponseDto> CheckCertifAsync(string email)
        {
            _logger.LogInformation("CheckCertifAsync called for email={Email}", email);
            try
            {
                var client = await CreateClientWithTokenAsync();
                client.DefaultRequestHeaders.Remove("email");
                client.DefaultRequestHeaders.Add("email", email);

                var url = $"{BaseUrl}/check_certif";
                _logger.LogDebug("Sending GET request to {Url} with email={Email}", url, email);

                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Received response StatusCode={StatusCode} Body={ResponseBody}", response.StatusCode, content);

                response.EnsureSuccessStatusCode();

                var dto = JsonSerializer.Deserialize<CheckCertifResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
                _logger.LogInformation("CheckCertifAsync completed for email={Email} with responseCode={ResponseCode}", email, dto?.ResponseCode);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckCertifAsync for email={Email}", email);
                throw;
            }
        }

        public async Task<VerifResponseDto> VerifSignatureAsync(string idDemand)
        {
            _logger.LogInformation("VerifSignatureAsync called for idDemand={IdDemand}", idDemand);
            try
            {
                var client = await CreateClientWithTokenAsync();
                client.DefaultRequestHeaders.Remove("IdDemand");
                client.DefaultRequestHeaders.Add("IdDemand", idDemand);

                var url = $"{BaseUrl}/verif";
                _logger.LogDebug("Sending GET request to {Url} with IdDemand={IdDemand}", url, idDemand);

                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Received response StatusCode={StatusCode} Body={ResponseBody}", response.StatusCode, content);

                response.EnsureSuccessStatusCode();

                var dto = JsonSerializer.Deserialize<VerifResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
                _logger.LogInformation("VerifSignatureAsync completed for idDemand={IdDemand} signatureComplete={SignatureComplete}", idDemand, dto?.SignatureComplete);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in VerifSignatureAsync for idDemand={IdDemand}", idDemand);
                throw;
            }
        }

        public async Task<SignatureResponseDto> CreateSignatureDemandAsync(SignatureRequestDto request)
        {
            _logger.LogInformation("CreateSignatureDemandAsync called for IdDemand={IdDemand} Service={Service}", request?.IdDemand, request?.Service);
            try
            {
                var client = await CreateClientWithTokenAsync();

                var jsonContent = JsonSerializer.Serialize(request);
                _logger.LogDebug("Posting to {BaseUrl} payload={Payload}", BaseUrl, jsonContent);

                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(BaseUrl, httpContent);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Received response StatusCode={StatusCode} Body={ResponseBody}", response.StatusCode, content);

                response.EnsureSuccessStatusCode();

                var dto = JsonSerializer.Deserialize<SignatureResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
                _logger.LogInformation("CreateSignatureDemandAsync succeeded for IdDemand={IdDemand} responseCode={ResponseCode}", request?.IdDemand, dto?.ResponseCode);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateSignatureDemandAsync for IdDemand={IdDemand}", request?.IdDemand);
                throw;
            }
        }


        // Méthode pour simuler la récupération des données utilisateur
        private (string Email, string FullName) GetUserDataByMatricule(string matricule)
        {
            _logger.LogDebug("GetUserDataByMatricule called for matricule={Matricule}", matricule);
            // Dans une application réelle, vous interrogeriez une base de données
            // ou un autre service (ex: Active Directory, API RH).
            // Ici, nous utilisons des données statiques pour l'exemple.
            if (matricule == "4591P")
            {
                var result = ("makrem.taieb@stb.com.tn", "Makrem Taieb");
                _logger.LogDebug("User found for matricule={Matricule} email={Email}", matricule, result.Item1);
                return result;
            }
            // Ajoutez d'autres utilisateurs si nécessaire
            // else if (matricule == "...") { ... }

            // Retourner une valeur par défaut ou lever une exception si l'utilisateur n'est pas trouvé
            _logger.LogWarning("No user found for matricule={Matricule}", matricule);
            throw new KeyNotFoundException($"Aucun utilisateur trouvé pour le matricule : {matricule}");
        }

        // Cette méthode est maintenant supprimée
        // private (string Email, string FullName ) GetUserDataByMatricule(string matricule) { ... }

        public async Task<SignatureResponseDto> CreateSignatureSimplifieeAsync(SignatureSimplifieeRequestDto request)
        {
            //if (request.IdDigital != "1234")
            //{
            //    return new SignatureResponseDto { Status = "error", ResponseCode = "AUTH-01", Message = "IdDigital non autorisé." };
            //}

            (string? email, string? fullName) userData;
            try
            {
                // Appel au nouveau service
                userData = await _graphApiService.GetUserDataByMatriculeAsync(request.Matricule!);
                // consommer la methode CheckCertifAsync (email) pour verifier si l'email existe dans la base de donnée de DigiGO
                var certifExists = await CheckCertifAsync(userData.email);
                if (certifExists.ResponseCode != "00")
                {
                    return new SignatureResponseDto { Status = "error", ResponseCode = "USER-03", Message = "L'email de l'utilisateur n'existe pas dans la base de donnée de DigiGO." };
                }
            }

            catch (KeyNotFoundException ex)
            {
                return new SignatureResponseDto { Status = "error", ResponseCode = "USER-02", Message = ex.Message };
            }
            catch (Exception ex) // Attraper d'autres erreurs potentielles (ex: API Graph indisponible)
            {
                return new SignatureResponseDto { Status = "error", ResponseCode = "GRAPH-500", Message = $"Erreur lors de la récupération des données utilisateur : {ex.Message}" };
            }

            var fullRequest = new SignatureRequestDto
            {
                IdDemand = request.IdDemand,
                Service = request.Service,
                FileName = request.FileName,
                Document = request.Document,
                Signatories = new List<SignatoryDto>
                {
                    new SignatoryDto
                    {
                        Mail = userData.email,
                        FullName = userData.fullName,
                        Company = "STB",
                        Department = "IT",
                        Type = "Client"
                    }
                },
                Settings = new SettingsDto
                {
                    Order = "Seq",
                    Rappel = new RappelDto
                    {
                        Frequency = "2",
                        ReminderTime = "6"
                    }
                }
            };

            return await CreateSignatureDemandAsync(fullRequest);
        }

    }
}