namespace FlightsDiggingApp.Models.Amadeus
{
    public class AuthResponse
    {
        public string type { get; set; }
        public string username { get; set; }
        public string application_name { get; set; }
        public string client_id { get; set; }
        public string token_type { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string approved { get; set; }
        public string scope { get; set; }
        public OperationStatus status { get; set; }
    }
}
