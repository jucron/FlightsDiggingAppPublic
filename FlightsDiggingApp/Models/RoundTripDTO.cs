using FlightsDiggingApp.Models.Amadeus;
using static FlightsDiggingApp.Models.Amadeus.AmadeusSearchFlightsResponse;

namespace FlightsDiggingApp.Models
{
    public class RoundTripDTO
    {
        public string Id { get; set; }
        public string source { set; get; }
        public string link { set; get; }
        public bool isOneWay { set; get; }
        public int numberOfBookableSeats { set; get; }
        public Price price { set; get; }
        public Duration totalDuration { set; get; }
        public int maxStops { set; get; }
        public FlightDTO departureFlight { set; get; }
        public FlightDTO returnFlight { set; get; }

        public class Duration
        {
            public int hours { get; set; }
            public int Minutes { get; set; }
        }

        public class Price
        {
            public string currency { set; get; }
            public double total { set; get; }
        }
    }
}
