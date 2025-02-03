using static FlightsDiggingApp.Models.GetAirportsResponse;

namespace FlightsDiggingApp.Models
{
    public class GetAirportsResponseDTO
    {
        public List<AirportOption> AirportOptions { get; set; }

        public class AirportOption
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        public OperationStatus status { get; set; }
    }
}
