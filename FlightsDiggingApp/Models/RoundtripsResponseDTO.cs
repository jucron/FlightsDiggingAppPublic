using FlightsDiggingApp.Models.RapidApi;

namespace FlightsDiggingApp.Models
{
    public class RoundtripsResponseDTO
    {
        public Guid id { get; set; }
        public RapidApiRoundtripsResponse data { get; set; }
        public OperationStatus status { get; set; }
        public RoundTripsMetrics metrics { get; set; }
        
    }
}
