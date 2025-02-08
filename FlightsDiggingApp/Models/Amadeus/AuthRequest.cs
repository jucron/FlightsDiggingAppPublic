namespace FlightsDiggingApp.Models.Amadeus
{
    public class AuthRequest
    {
        public required string grant_type { get; set; }
        public required string client_id { get; set; }
        public required string client_secret { get; set; }
    }
}
