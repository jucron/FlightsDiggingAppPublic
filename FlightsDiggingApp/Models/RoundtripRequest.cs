using FlightsDiggingApp.Models.RapidApi;
using static FlightsDiggingApp.Models.RapidApi.RapidApiAirportsResponse;

namespace FlightsDiggingApp.Models
{
    public class RoundtripRequest
    {
        public string from { get; set; }
        public string to { get; set; }
        public string currency { get; set; }
        public int adults { get; set; }
        public int children { get; set; }
        public int infants { get; set; }
        public string travelClass { get; set; }
        public string departDate { get; set; }
        public string returnDate { get; set; }
        public Filter filter { get; set; }

    }
}
