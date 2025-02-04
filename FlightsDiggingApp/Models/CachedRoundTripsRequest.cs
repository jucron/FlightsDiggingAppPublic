namespace FlightsDiggingApp.Models
{
    public class CachedRoundTripsRequest
    {
        public Filter filter {  get; set; }
        public List<Guid> ids { get; set; }
    }
}
