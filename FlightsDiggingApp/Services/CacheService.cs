using System;
using FlightsDiggingApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FlightsDiggingApp.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Guid GenerateUUID()
        {
            return Guid.NewGuid();
        }

        public RoundtripsResponseDTO RetrieveGetRoundtripsResponseDTO(Guid uuid)
        {
            var errorMessage = "Could not retrieve response from cache. Might have expired.";
            var errorResponse = new RoundtripsResponseDTO() {status = OperationStatus.CreateStatusFailure(errorMessage) };
            
            _cache.TryGetValue<RoundtripsResponseDTO>(uuid, out var response);
            
            return (response != null) ? response : errorResponse;
        }


        public bool StoreGetRoundtripsResponseDTO(RoundtripsResponseDTO getRoundtripsResponseDTO, TimeSpan? expiration = null)
        {

            Guid uuid = getRoundtripsResponseDTO.id;
            if (uuid == Guid.Empty)
            {
                return false;
            }
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            };

            _cache.Set(uuid, getRoundtripsResponseDTO, cacheEntryOptions);
            return true;
        }

    }
}
