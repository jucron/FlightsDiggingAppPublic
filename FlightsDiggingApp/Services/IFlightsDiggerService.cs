using System.Net.WebSockets;
using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services
{
    public interface IFlightsDiggerService
    {
        public AirportsResponseDTO GetAirports(string query);
        CachedRoundTripsResponseDTO getCachedRoundTrips(CachedRoundTripsRequest request);
        public Task HandleRoundTripsAsync(WebSocket webSocket);

        public string GetAuthToken();
    }
}
