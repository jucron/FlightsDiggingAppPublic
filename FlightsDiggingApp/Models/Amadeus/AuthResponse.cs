namespace FlightsDiggingApp.Models.Amadeus
{
    public class AuthResponse
    {
        public required string type { get; set; }
        public required string username { get; set; }
        public required string application_name { get; set; }
        public required string client_id { get; set; }
        public required string token_type { get; set; }
        public required string access_token { get; set; }
        public required string expires_in { get; set; }
        public required string approved { get; set; }
        public required string scope { get; set; }
        public required OperationStatus status { get; set; }
    }
}
