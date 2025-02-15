using FlightsDiggingApp.Models.Amadeus;
using static FlightsDiggingApp.Models.Amadeus.AmadeusSearchFlightsResponse;

namespace FlightsDiggingApp.Models
{
    public class FlightDTO
    {
        public string Id { get; set; }
        public string link { set; get; }
        public bool isOneWay { set; get; }
        public int numberOfBookableSeats { set; get; }
        public Price price { set; get; }
        public Duration duration { set; get; }
        public int stops { set; get; }

        public List<Segment> segments { set; get; }

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
