using static FlightsDiggingApp.Models.GetAirportsResponse;

namespace FlightsDiggingApp.Models
{
    public record GetRoundtripsResponse
    {
        public string from;
        public string to;
        public string departDate;
        public string returnDate;
        public string currency;

        public List<Flight> flights;

        public Status status { get; set; }
        public record Status
        {
            public bool hasError;
            public string errorDescription;
        }
        public record Flight
        {
            public double rawPrice;
            public int hours;
            public string stops;
            public string company;

        }
    }
    
}
