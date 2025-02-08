using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services
{
    public interface ICacheService
    {
        public bool StoreGetRoundtripsResponseDTO(RoundtripsResponseDTO getRoundtripsResponseDTO, TimeSpan? expiration = null);

        public RoundtripsResponseDTO RetrieveGetRoundtripsResponseDTO(Guid guid);

        public Guid GenerateUUID();

        public string GetToken();
        public void SetToken(string token);
    }
}
