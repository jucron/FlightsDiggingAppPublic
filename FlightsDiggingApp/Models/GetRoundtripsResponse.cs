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
        public string link;
        public int adults;
        public List<Child> children;
        public int infants;
        public string cabinclass;

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
            public List<Company> companies;

        }

        public record Company
        {
            public string logoUrl;
            public string name;
        }
        public record Child 
        {
            public int age;
        }
    }
    
}
