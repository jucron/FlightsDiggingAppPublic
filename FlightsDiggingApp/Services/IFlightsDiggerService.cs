using System.Net.WebSockets;
using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services
{
    public interface IFlightsDiggerService
    {
        public Task<GetAirportsResponse> GetAirports(string query);   
        public Task HandleRoundTripsAsync(WebSocket webSocket);
    }
}
