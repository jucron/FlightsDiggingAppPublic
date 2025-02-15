namespace FlightsDiggingApp.Models
{
    public class RoundTripMetrics
    {
        public int totalFlightsFiltered { get; set; }
        public int totalFlightsOriginal { get; set; }
        public double maxPrice { get; set; }
        public double minPrice { get; set; }
        public int maxHours { get; set; }
        public int minHours { get; set; }

    }
}
