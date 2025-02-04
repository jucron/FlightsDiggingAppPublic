namespace FlightsDiggingApp.Models
{
    public class RoundtripsResponseDTO
    {
        public Guid id { get; set; }
        public RoundtripsResponse data { get; set; }
        public OperationStatus status { get; set; }
        
    }
}
