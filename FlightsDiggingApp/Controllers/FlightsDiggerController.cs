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
        private readonly RapidApiService _apiService;
        private readonly RoundTripsService _roundTripsService;

        public FlightsDiggerController(ILogger<FlightsDiggerController> logger)
        {
            _logger = logger;
            _apiService = new ApiService(logger);
            _roundTripsService = new RoundTripsService(logger, _apiService);
        }

        [HttpGet("getroundtrips")]
        public async Task GetRoundTrips()
        {
            _logger.LogInformation(">>>>>>>>>>Starting: GetRoundTrips");
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _roundTripsService.HandleRoundTripsAsync(webSocket);
            }
            else
            {
                _logger.LogInformation("Not a WebSocket request");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            
        }

        [HttpGet("airports")]
        public GetAirportsResponseDTO GetAirports([FromQuery] string query)
        {
            return GetAirportsMapper.MapGetAirportsResponseToDTO(_apiService.GetAirportsAsync(query).Result);
        }
    }
}
