namespace FlightsDiggingApp.Models
{
    public class GetAirportsResponse
    {
        public List<Datum> data { get; set; }
        public bool status { get; set; }
        public class Datum
        {
            public Presentation presentation { get; set; }
            public Navigation navigation { get; set; }
        }

        public class Navigation
        {
            public string entityId { get; set; }
            public string entityType { get; set; }
            public string localizedName { get; set; }
            public RelevantFlightParams relevantFlightParams { get; set; }
            public RelevantHotelParams relevantHotelParams { get; set; }
        }

        public class Presentation
        {
            public string title { get; set; }
            public string suggestionTitle { get; set; }
            public string subtitle { get; set; }
            public string id { get; set; }
        }

        public class RelevantFlightParams
        {
            public string skyId { get; set; }
            public string entityId { get; set; }
            public string flightPlaceType { get; set; }
            public string localizedName { get; set; }
        }

        public class RelevantHotelParams
        {
            public string entityId { get; set; }
            public string entityType { get; set; }
            public string localizedName { get; set; }
        }

        public OperationStatus operationStatus { get; set; }
    }
}
