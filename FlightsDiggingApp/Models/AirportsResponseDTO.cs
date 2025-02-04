using static FlightsDiggingApp.Models.AirportsResponse;

namespace FlightsDiggingApp.Models
{
    public class AirportsResponseDTO
    {
        public List<AirportOption> AirportOptions { get; set; }
        public OperationStatus status { get; set; }

        public class AirportOption
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        
    }
}
