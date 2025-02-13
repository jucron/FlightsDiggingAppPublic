using static FlightsDiggingApp.Models.RapidApi.RapidApiAirportsResponse;

namespace FlightsDiggingApp.Models
{
    public class AirportsResponseDTO
    {
        public List<AirportOption> AirportOptions { get; set; }
        public OperationStatus status { get; set; }

        public class AirportOption
        {
            public string iataCode { get; set; }
            public string airport { get; set; }
            public string city { get; set; }
            public string country { get; set; }
        }
        
    }
}
