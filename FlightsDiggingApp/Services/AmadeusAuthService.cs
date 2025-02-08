
using FlightsDiggingApp.Mappers;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Models.Amadeus;
using System.Text;
using System.Text.Json;

namespace FlightsDiggingApp.Services
{
    public class AmadeusAuthService : IAuthService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<AmadeusAuthService> _logger;

        public AmadeusAuthService(ICacheService cacheService, ILogger<AmadeusAuthService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public string getToken()
        {
            // Get first from cache
            var token = _cacheService.GetToken();
            if (token != "")
            {
                return token;
            }

            // If not in cache, we need to fetch from server
            token = GetAsyncToken().Result;
            _cacheService.SetToken(token);
            return token;
        }

        private async Task<string> GetAsyncToken()
        {

            var authRequest = new AuthRequest
            {
                grant_type = "client_credentials",
                client_id = "UEBTNCyAuKZ6FDThuAhKUqIYEjzQDIkq",
                client_secret = "wkJB2poI89bHXKvn"
            };

            // Convert AuthRequest object to JSON
            var jsonRequest = JsonSerializer.Serialize(authRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            _logger.LogInformation($"GetAsyncToken");
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://test.api.amadeus.com/v1/security/oauth2/token"),
                Headers =
                {
                    { "Content-Type", "application/x-www-form-urlencoded" },

                },
                Content = content
            };


            try
            {
                using var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                if (jsonString != null)
                {
                    AuthResponse finalResponse = JsonSerializer.Deserialize<AuthResponse>(jsonString);
                    if (finalResponse != null)
                    {
                        finalResponse.status = OperationStatus.CreateStatusSuccess();
                        return finalResponse.access_token;
                    }
                }
                _logger.LogInformation("Unexpected error -> jsonstring from response API is null");
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in GetAsyncToken " + ex.ToString());
                return "";
            }
        }
    }
}
