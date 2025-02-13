namespace FlightsDiggingApp.Models.Amadeus
{
    public class AmadeusErrors
    {
        public int status { get; set; }
        public int code { get; set; }
        public string title { get; set; }
        public string detail { get; set; }
        public Source source { get; set; }
    }
    public class Source
    {
        public string parameter { get; set; }
        public string example { get; set; }
    }
}
