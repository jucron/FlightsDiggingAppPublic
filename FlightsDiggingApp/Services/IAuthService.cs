namespace FlightsDiggingApp.Services
{
    public interface IAuthService
    {
        void ClearToken();
        public string GetToken();
    }
}
