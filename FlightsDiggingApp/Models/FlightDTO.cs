using FlightsDiggingApp.Models.Amadeus;
using static FlightsDiggingApp.Models.Amadeus.AmadeusSearchFlightsResponse;

namespace FlightsDiggingApp.Models
{
    public class FlightDTO
    {
        public RoundTripDTO.Duration duration { set; get; }
        public int stops { set; get; }
        public List<Segment> segments { set; get; }
   
    }
}
