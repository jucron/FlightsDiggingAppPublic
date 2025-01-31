using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using FlightsDiggingApp.Mappers;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FlightsDiggingApp.Controllers
{
    [EnableCors("AllowAll")]
    [ApiController]
    [Route("api/flightsdigger")]
    public class FlightsDiggerController : ControllerBase
    {
        private readonly ILogger<FlightsDiggerController> _logger;
        private readonly ApiService _apiService;
        private readonly GetRoundTripsService _getRoundTripsService;

        public FlightsDiggerController(ILogger<FlightsDiggerController> logger)
        {
            _logger = logger;
            _apiService = new ApiService(logger);
            _getRoundTripsService = new GetRoundTripsService(logger, _apiService);
        }


        [HttpGet("getstaticflights")]
        public string GetFlightsStatic(CancellationToken cancellationToken)
        {
            StringBuilder resultToReturn = new StringBuilder();

            
            var from = "GIG";
            var to = "OPO";
            var currency = "BRL";
            var limitFlightHour = 16;

            var initDepartDateString = "2025-06-05";
            var endDepartDateString = "2025-06-10";

            var initReturnDateString = "2025-06-25";
            var endReturnDateString = "2025-06-28";
            

            DateTime startDepartDate = DateTime.Parse(initDepartDateString);
            DateTime endDepartDate = DateTime.Parse(endDepartDateString);

            DateTime startReturnDate = DateTime.Parse(initReturnDateString);
            DateTime endReturnDate = DateTime.Parse(endReturnDateString);

            for (DateTime departDate = startDepartDate; departDate <= endDepartDate; departDate = departDate.AddDays(1))
            {

                for (DateTime returnDate = startReturnDate; returnDate <= endReturnDate; returnDate = returnDate.AddDays(1))
                {
                    // Check if process is cancelled by Client
                    cancellationToken.ThrowIfCancellationRequested();

                    // Use the date in your format
                    string departDateString = departDate.ToString("yyyy-MM-dd");
                    string returnDateString = returnDate.ToString("yyyy-MM-dd");

                    string headline = $"From {from} to {to}: Departure: {departDateString} Return date: {returnDateString}.";
                    
                    _logger.LogInformation(">>>>>>>>>>Starting: "+headline);

                    resultToReturn.Append(headline);
                    resultToReturn.Append('\n');
                    

                    string flights = _apiService.getRoundTrip(from, to, currency, departDateString, returnDateString, limitFlightHour, cancellationToken );

                    resultToReturn.Append(flights);
                    resultToReturn.Append('\n');
                }

            }
            return resultToReturn.ToString();
        }

        [HttpGet("getroundtrips")]
        public async Task GetRoundTrips()
        {
            _logger.LogInformation(">>>>>>>>>>Starting: GetRoundTrips");
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _getRoundTripsService.HandleRoundTripsAsync(webSocket);
            }
            else
            {
                _logger.LogInformation("Not a WebSocket request");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            
        }

        private async Task StreamGetRoundTripsAsync(WebSocket webSocket)
        {
            for (int i = 1; i <= 5; i++)
            {
                _logger.LogInformation(">>>>>>>>>>Starting sending: " + i);
                var message = $"Data chunk {i}";
                var buffer = Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                await Task.Delay(1500); // Simulate processing delay
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Streaming complete", CancellationToken.None);

        }

        [HttpGet("airports")]
        public GetAirportsResponseDTO GetAirports([FromQuery] string query)
        {
            return GetAirportsMapper.MapGetAirportsResponseToDTO(_apiService.GetAirportsAsync(query).Result);
        }
    }
}
