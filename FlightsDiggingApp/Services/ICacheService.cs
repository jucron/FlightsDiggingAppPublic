using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services
{
    public interface ICacheService
    {
        public Guid StoreGetRoundtripsResponseDTO(GetRoundtripsResponseDTO getRoundtripsResponseDTO, TimeSpan? expiration = null);

        public GetRoundtripsResponseDTO RetrieveGetRoundtripsResponseDTO(Guid guid);
    }
}
