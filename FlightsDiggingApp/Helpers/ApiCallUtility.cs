using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Models.Amadeus;
using static System.Net.WebRequestMethods;

namespace FlightsDiggingApp.Helpers
{
    public static class ApiCallUtility
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<ApiCallResponse<TResponse>> GetAsync<TResponse>(
            string url, 
            Dictionary<string, string>? parameters = null,
            Dictionary<string, string>? headers = null,
            string? bearerToken = null)
        {
            url = AddParametersToUrl(url, parameters);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };

            AddBearerTokenIfProvided(request,bearerToken);

            AddHeadersToRequest(request, headers);

            try
            {
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode(); // Throws if not 2xx

                var jsonString = await response.Content.ReadAsStringAsync();
                var data = !string.IsNullOrEmpty(jsonString)
                    ? JsonSerializer.Deserialize<TResponse>(jsonString)
                    : default;

                return new ApiCallResponse<TResponse>
                {
                    status = OperationStatus.CreateStatusSuccess(),
                    data = data
                };
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error: {ex.Message}";
                errorMessage = (ex is HttpRequestException) ? "HTTP " + errorMessage : "Unexpected " + errorMessage;

                return new ApiCallResponse<TResponse>
                {
                    status = OperationStatus.CreateStatusFailure(errorMessage),
                    data = default
                };
            }
        }

        public static async Task<ApiCallResponse<TResponse>> PostAsync<TResponse>(string url, Object payload, Dictionary<string, string>? headers = null, string? bearerToken = null)
        {

            // Convert AuthRequest object to JSON
            var jsonRequest = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = content
            };

            AddBearerTokenIfProvided(request, bearerToken);

            AddHeadersToRequest(request, headers);

            try
            {
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode(); // Throws if not 2xx

                var jsonString = await response.Content.ReadAsStringAsync();
                var data = !string.IsNullOrEmpty(jsonString)
                    ? JsonSerializer.Deserialize<TResponse>(jsonString)
                    : default;

                return new ApiCallResponse<TResponse>
                {
                    status = OperationStatus.CreateStatusSuccess(),
                    data = data
                };
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error: {ex.ToString()} \nrequest: {request.ToString()}\ncontent: {content.ReadAsStringAsync()}";
                errorMessage = (ex is HttpRequestException) ? "HTTP " + errorMessage : "Unexpected " + errorMessage;

                return new ApiCallResponse<TResponse>
                {
                    status = OperationStatus.CreateStatusFailure(errorMessage),
                    data = default
                };
            }
        }
        public static async Task<ApiCallResponse<TResponse>> PostAsyncFormUrlEncodedContent<TResponse>(string url, Dictionary<string, string> parameters, Dictionary<string, string>? headers = null)
        {
            var content = new FormUrlEncodedContent(parameters);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = content
            };

            AddHeadersToRequest(request, headers);

            try
            {
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode(); // Throws if not 2xx

                var jsonString = await response.Content.ReadAsStringAsync();
                var data = !string.IsNullOrEmpty(jsonString)
                    ? JsonSerializer.Deserialize<TResponse>(jsonString)
                    : default;

                return new ApiCallResponse<TResponse>
                {
                    status = OperationStatus.CreateStatusSuccess(),
                    data = data
                };
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error: {ex.ToString()} \nrequest: {request.ToString()}\ncontent: {content.ReadAsStringAsync()}";
                errorMessage = (ex is HttpRequestException) ? "HTTP " + errorMessage : "Unexpected " + errorMessage;

                return new ApiCallResponse<TResponse>
                {
                    status = OperationStatus.CreateStatusFailure(errorMessage),
                    data = default
                };
            }
        }
        private static void AddHeadersToRequest(HttpRequestMessage request, Dictionary<string, string>? headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }
        private static void AddBearerTokenIfProvided(HttpRequestMessage request, string? bearerToken)
        {
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }
        }
        private static string AddParametersToUrl(string url, Dictionary<string, string>? parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                var queryString = string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                url = $"{url}?{queryString}";
            }
            return url;
        }
    }
}
