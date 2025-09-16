namespace APIs_Signature_DigiGO.Iservices
{
    public interface ITokenService
    {
        Task<string> GetApiTokenAsync();
    }
}
