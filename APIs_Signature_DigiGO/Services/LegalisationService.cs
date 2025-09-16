using APIs_Signature_DigiGO.Dtos;
using APIs_Signature_DigiGO.Iservices;
using System.Text.Json;

namespace APIs_Signature_DigiGO.Services
{
    public class LegalisationService : ILegalisationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenService _tokenService;
        private const string BaseUrl = "https://legalisation.stb.com.tn/Signer/signature_document";

        public LegalisationService(IHttpClientFactory httpClientFactory, ITokenService tokenService)
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
        }

        private async Task<HttpClient> CreateClientWithTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var token = await _tokenService.GetApiTokenAsync();
            // Le token est ajouté comme un header standard, comme dans Postman
            client.DefaultRequestHeaders.Add("token", token);
            return client;
        }

        public async Task<CheckDemandResponseDto> CheckDemandAsync(string demandeId)
        {
            var client = await CreateClientWithTokenAsync();
            client.DefaultRequestHeaders.Add("demandeId", demandeId);

            var response = await client.GetAsync($"{BaseUrl}/check_demand");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CheckDemandResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<CheckCertifResponseDto> CheckCertifAsync(string email)
        {
            var client = await CreateClientWithTokenAsync();
            client.DefaultRequestHeaders.Add("email", email);

            var response = await client.GetAsync($"{BaseUrl}/check_certif");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CheckCertifResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<VerifResponseDto> VerifSignatureAsync(string idDemand)
        {
            var client = await CreateClientWithTokenAsync();
            client.DefaultRequestHeaders.Add("IdDemand", idDemand);

            var response = await client.GetAsync($"{BaseUrl}/verif");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<VerifResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<SignatureResponseDto> CreateSignatureDemandAsync(SignatureRequestDto request)
        {
            var client = await CreateClientWithTokenAsync();

            var jsonContent = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(BaseUrl, httpContent);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SignatureResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }


        // Méthode pour simuler la récupération des données utilisateur
        private (string Email, string FullName) GetUserDataByMatricule(string matricule)
        {
            // Dans une application réelle, vous interrogeriez une base de données
            // ou un autre service (ex: Active Directory, API RH).
            // Ici, nous utilisons des données statiques pour l'exemple.
            if (matricule == "4591P")
            {
                return ("makrem.taieb@stb.com.tn", "Makrem Taieb");
            }
            // Ajoutez d'autres utilisateurs si nécessaire
            // else if (matricule == "...") { ... }

            // Retourner une valeur par défaut ou lever une exception si l'utilisateur n'est pas trouvé
            throw new KeyNotFoundException($"Aucun utilisateur trouvé pour le matricule : {matricule}");
        }

        // Nouvelle implémentation pour la signature simplifiée
        public async Task<SignatureResponseDto> CreateSignatureSimplifieeAsync(SignatureSimplifieeRequestDto request)
        {
            // 1. Vérifier si l'IdDigital est éligible
            //if (request.IdDigital != "1234")
            //{
            //    // Retourner une réponse d'erreur claire
            //    return new SignatureResponseDto
            //    {
            //        Status = "error",
            //        ResponseCode = "AUTH-01", // Code d'erreur personnalisé
            //        Message = "IdDigital non autorisé à utiliser ce service."
            //    };
            //}

            // 2. Récupérer les informations du signataire à partir du matricule
            (string email, string fullName) userData;
            try
            {
                userData = GetUserDataByMatricule(request.Matricule!);
            }
            catch (KeyNotFoundException ex)
            {
                return new SignatureResponseDto
                {
                    Status = "error",
                    ResponseCode = "USER-02",
                    Message = ex.Message
                };
            }

            // 3. Construire l'objet de requête complet (SignatureRequestDto)
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
                        Company = "STB",      // Valeur figée
                        Department = "IT",    // Valeur figée
                        Type = "Client"       // Valeur figée
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

            // 4. Appeler la méthode de signature originale avec l'objet complet
            return await CreateSignatureDemandAsync(fullRequest);
        }


    }
}