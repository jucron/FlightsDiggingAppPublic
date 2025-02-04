namespace FlightsDiggingApp.Models
{
    public class Filter
    {
        public double maxPrice { get; set; }
        public double minPrice { get; set; } = 0;
        public int maxDuration { get; set; }
        public int minDuration { get; set; }
        public int maxStops { get; set; }
        public int maxFlights { get; set; }
    }
}
