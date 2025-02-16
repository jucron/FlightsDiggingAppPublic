namespace FlightsDiggingApp.Models
{
    public class CachedRoundTripsResponseDTO
    {
        public List<RoundtripResponseDTO> responses {get; set; }
        public OperationStatus status { get; set; }
    }
}
