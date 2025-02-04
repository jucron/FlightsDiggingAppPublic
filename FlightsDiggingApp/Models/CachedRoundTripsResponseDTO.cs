namespace FlightsDiggingApp.Models
{
    public class CachedRoundTripsResponseDTO
    {
        public List<RoundtripsResponseDTO> responses {get; set; }
        public OperationStatus status { get; set; }
    }
}
