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

        public GetRoundtripsResponseDTO RetrieveGetRoundtripsResponseDTO(Guid uuid)
        {
            var errorMessage = "Could not retrieve response from cache. Might have expired.";
            var errorResponse = new GetRoundtripsResponseDTO() {status = OperationStatus.CreateStatusFailure(errorMessage) };
            
            _cache.TryGetValue<GetRoundtripsResponseDTO>(uuid, out var response);
            
            return (response != null) ? response : errorResponse;
        }


        public Guid StoreGetRoundtripsResponseDTO(GetRoundtripsResponseDTO getRoundtripsResponseDTO, TimeSpan? expiration = null)
        {
            Guid uuid = Guid.NewGuid(); // Generate a unique ID for this response
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            };

            _cache.Set(uuid, getRoundtripsResponseDTO, cacheEntryOptions);
            return uuid;
        }

        public Guid StoreGetRoundtripsResponseDTO(GetRoundtripsResponseDTO getRoundtripsResponseDTO)
        {
            throw new NotImplementedException();
        }
    }
}
