using FlightsDiggingApp.Models.RapidApi;
using static FlightsDiggingApp.Models.Amadeus.AmadeusSearchFlightsResponse;

namespace FlightsDiggingApp.Models
{
    public class RoundtripResponseDTO
    {
        public Guid id { get; set; }
        public List<FlightDTO> data { get; set; }
        public RoundtripRequest request { get; set; }
        public Dictionaries dictionaries { get; set; }
        public RoundTripMetrics metrics { get; set; }
        public OperationStatus status { get; set; }
        
    }
}
