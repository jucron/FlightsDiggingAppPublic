namespace FlightsDiggingApp.Models
{
    public class SearchRoundTripResponse
    {
        public Data data { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public class Context
        {
            public string status { get; set; }
            public string sessionId { get; set; }
            public int totalResults { get; set; }
            public int filterTotalResults { get; set; }
        }

        public class Data
        {
            public Context context { get; set; }
            public List<SearchIncompleteResponse.Itinerary> itineraries { get; set; }
            public List<object> messages { get; set; }
            public FilterStats filterStats { get; set; }
            public string flightsSessionId { get; set; }
            public string destinationImageUrl { get; set; }
            public string token { get; set; }
        }

        public class Direct
        {
            public bool isPresent { get; set; }
        }

        public class Duration
        {
            public int min { get; set; }
            public int max { get; set; }
            public int multiCityMin { get; set; }
            public int multiCityMax { get; set; }
        }

        public class FilterStats
        {
            public Duration duration { get; set; }
            public List<object> airports { get; set; }
            public List<object> carriers { get; set; }
            public StopPrices stopPrices { get; set; }
        }

        public class One
        {
            public bool isPresent { get; set; }
        }

        public class StopPrices
        {
            public Direct direct { get; set; }
            public One one { get; set; }
            public TwoOrMore twoOrMore { get; set; }
        }

        public class TwoOrMore
        {
            public bool isPresent { get; set; }
        }


    }
}
