using System;
using System.Net;
using FlightsDiggingApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FlightsDiggingApp.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly string TOKEN_KEY = "token-key";

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void ClearToken()
        {
            _cache.Remove(TOKEN_KEY);
        }

        public Guid GenerateUUID()
        {
            return Guid.NewGuid();
        }

        public string GetToken()
        {
            _cache.TryGetValue<string>(TOKEN_KEY, out var token);
            return token != null ? token : "";
        }

        public RoundtripResponseDTO RetrieveRoundtripResponseDTO(Guid uuid)
        {
            _cache.TryGetValue<RoundtripResponseDTO>(uuid, out var response);

            return (response != null) ? response : CreateResponseWithCacheError();
        }

        private RoundtripResponseDTO CreateResponseWithCacheError()
        {
            var errorMessage = "Could not retrieve response from cache. Might have expired.";
            return new RoundtripResponseDTO() { status = OperationStatus.CreateStatusFailure(HttpStatusCode.NotFound, errorMessage) };
        }

        public void SetToken(string token)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(TOKEN_KEY, token, cacheEntryOptions);
        }

        public bool StoreRoundtripResponseDTO(RoundtripResponseDTO responseDTO, TimeSpan? expiration = null)
        {
            Guid uuid = responseDTO.id;
            if (uuid == Guid.Empty)
            {
                return false;
            }
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            };

            _cache.Set(uuid, responseDTO, cacheEntryOptions);
            return true;
        }
    }
}
