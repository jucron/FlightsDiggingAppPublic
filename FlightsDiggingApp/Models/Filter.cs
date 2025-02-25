namespace FlightsDiggingApp.Models
{
    public class Filter
    {
        public double maxPrice { get; set; }
        public double minPrice { get; set; } = 0;
        public int maxDurationHours { get; set; }
        public int minDurationHours { get; set; }
        public int maxStops { get; set; }
        public int maxRoundTrips { get; set; }
        public bool isDirect { get; set; }
    }
}
