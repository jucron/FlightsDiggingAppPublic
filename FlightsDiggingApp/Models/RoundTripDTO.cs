
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
        public MinMax<int> durationStatsMinutes { set; get; }
        public int maxStops { set; get; }
        public FlightDTO departureFlight { set; get; }
        public FlightDTO returnFlight { set; get; }

        public class Duration
        {
            public int hours { set; get; }
            public int minutes { set; get; }
        }


        public class Price
        {
            public string currency { set; get; }
            public double total { set; get; }
        }
    }
}
