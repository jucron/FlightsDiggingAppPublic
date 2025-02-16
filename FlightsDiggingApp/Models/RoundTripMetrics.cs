namespace FlightsDiggingApp.Models
{
    public class RoundTripMetrics
    {
        public Metrics originalMetrics { get; set; }
        public Metrics filteredMetrics { get; set; }

        public class Metrics
        {
            public int totalFlights { get; set; }
            public double maxPrice { get; set; }
            public double minPrice { get; set; }
            public int maxHours { get; set; }
            public int minHours { get; set; }
        }
    }
}
