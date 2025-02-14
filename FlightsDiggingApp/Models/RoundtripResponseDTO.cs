using FlightsDiggingApp.Models.RapidApi;

namespace FlightsDiggingApp.Models
{
    public class RoundtripResponseDTO
    {
        public IApiServiceResponse data { get; set; }
        public OperationStatus status { get; set; }
        
    }
}
