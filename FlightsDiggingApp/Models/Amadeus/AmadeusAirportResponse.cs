namespace FlightsDiggingApp.Models.Amadeus
{
    public class AmadeusAirportResponse: IApiServiceResponse
    {
        public Meta meta { get; set; }
        public List<Datum> data { get; set; }
        public List<AmadeusErrors> errors { get; set; }
        public OperationStatus operationStatus { get; set; }

        public class Address
        {
            public string cityName { get; set; }
            public string cityCode { get; set; }
            public string countryName { get; set; }
            public string countryCode { get; set; }
            public string regionCode { get; set; }
        }

        public class Analytics
        {
            public Travelers travelers { get; set; }
        }

        public class Datum
        {
            public string type { get; set; }
            public string subType { get; set; }
            public string name { get; set; }
            public string detailedName { get; set; }
            public string id { get; set; }
            public Self self { get; set; }
            public string timeZoneOffset { get; set; }
            public string iataCode { get; set; }
            public GeoCode geoCode { get; set; }
            public Address address { get; set; }
            public Analytics analytics { get; set; }
        }

        public class GeoCode
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
        }

        public class Meta
        {
            public int count { get; set; }
            public Links links { get; set; }
        }

   

        public class Self
        {
            public string href { get; set; }
            public List<string> methods { get; set; }
        }

        public class Travelers
        {
            public int score { get; set; }
        }



    }
}
