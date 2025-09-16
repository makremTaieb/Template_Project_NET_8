namespace APIs_Signature_DigiGO.Iservices
{
    public interface IGraphApiService
    {
        Task<(string? Email, string? FullName)> GetUserDataByMatriculeAsync(string matricule);
    }
}