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
        private readonly IFlightsDiggerService _flightsDiggerService;

        public FlightsDiggerController(ILogger<FlightsDiggerController> logger, IFlightsDiggerService flightsDiggerService)
        {
            _logger = logger;
            _flightsDiggerService = flightsDiggerService;
        }

        [HttpGet("roundtrips")]
        public async Task RoundTrips()
        {
            _logger.LogInformation(">>>>>>>>>>Starting: RoundTrips");
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _flightsDiggerService.HandleRoundTripsAsync(webSocket);
            }
            else
            {
                _logger.LogInformation("Not a WebSocket request");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            
        }

        [HttpGet("airports")]
        public AirportsResponseDTO GetAirports([FromQuery] string query)
        {
            return _flightsDiggerService.GetAirports(query);
        }

        [HttpGet("roundtripsfromcache")]
        public CachedRoundTripsResponseDTO CachedRoundTrips([FromQuery] CachedRoundTripsRequest request)
        {
            return _flightsDiggerService.getCachedRoundTrips(request);
        }
    }
}
