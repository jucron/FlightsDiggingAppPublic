namespace FlightsDiggingApp.Models.RapidApi
{
    public class SearchIncompleteResponse
    {
        public Data data { get; set; }
        public bool status { get; set; }
        public string message { get; set; }

        public class Airport
        {
            public string city { get; set; }
            public List<Airport> airports { get; set; }
            public string id { get; set; }
            public string entityId { get; set; }
            public string name { get; set; }
        }

        public class Carriers
        {
            public List<Marketing> marketing { get; set; }
            public List<Operating> operating { get; set; }
            public string operationType { get; set; }
        }

        public class Carrier
        {
            public int id { get; set; }
            public string alternateId { get; set; }
            public string logoUrl { get; set; }
            public string name { get; set; }
            public List<Marketing> marketing { get; set; }
            public string operationType { get; set; }
            public List<Operating> operating { get; set; }
        }

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
            public List<Itinerary> itineraries { get; set; }
            public List<object> messages { get; set; }
            public FilterStats filterStats { get; set; }
            public string flightsSessionId { get; set; }
            public string destinationImageUrl { get; set; }
        }

        public class Destination
        {
            public string id { get; set; }
            public string entityId { get; set; }
            public string name { get; set; }
            public string displayCode { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public bool isHighlighted { get; set; }
            public string flightPlaceId { get; set; }
            public Parent parent { get; set; }
            public string type { get; set; }
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

        public class Eco
        {
            public double ecoContenderDelta { get; set; }
        }

        public class FareAttributes
        {
        }

        public class FarePolicy
        {
            public bool isChangeAllowed { get; set; }
            public bool isPartiallyChangeable { get; set; }
            public bool isCancellationAllowed { get; set; }
            public bool isPartiallyRefundable { get; set; }
        }

        public class FilterStats
        {
            public Duration duration { get; set; }
            public List<Airport> airports { get; set; }
            public List<Carrier> carriers { get; set; }
            public StopPrices stopPrices { get; set; }
        }

        public class Itinerary
        {
            public string id { get; set; }
            public Price price { get; set; }
            public List<Leg> legs { get; set; }
            public bool isSelfTransfer { get; set; }
            public bool isProtectedSelfTransfer { get; set; }
            public FarePolicy farePolicy { get; set; }
            public Eco eco { get; set; }
            public FareAttributes fareAttributes { get; set; }
            public List<string> tags { get; set; }
            public bool isMashUp { get; set; }
            public bool hasFlexibleOptions { get; set; }
            public double score { get; set; }
        }

        public class Leg
        {
            public string id { get; set; }
            public Origin origin { get; set; }
            public Destination destination { get; set; }
            public int durationInMinutes { get; set; }
            public int stopCount { get; set; }
            public bool isSmallestStops { get; set; }
            public DateTime departure { get; set; }
            public DateTime arrival { get; set; }
            public int timeDeltaInDays { get; set; }
            public Carriers carriers { get; set; }
            public List<Segment> segments { get; set; }
            public List<string> airportChangesIn { get; set; }
        }

        public class Marketing
        {
            public int id { get; set; }
            public string alternateId { get; set; }
            public string logoUrl { get; set; }
            public string name { get; set; }
        }

        public class MarketingCarrier
        {
            public int id { get; set; }
            public string name { get; set; }
            public string alternateId { get; set; }
            public int allianceId { get; set; }
            public string displayCode { get; set; }
        }

        public class One
        {
            public bool isPresent { get; set; }
            public string formattedPrice { get; set; }
        }

        public class Operating
        {
            public int id { get; set; }
            public string alternateId { get; set; }
            public string logoUrl { get; set; }
            public string name { get; set; }
        }

        public class OperatingCarrier
        {
            public int id { get; set; }
            public string name { get; set; }
            public string alternateId { get; set; }
            public int allianceId { get; set; }
            public string displayCode { get; set; }
        }

        public class Origin
        {
            public string id { get; set; }
            public string entityId { get; set; }
            public string name { get; set; }
            public string displayCode { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public bool isHighlighted { get; set; }
            public string flightPlaceId { get; set; }
            public Parent parent { get; set; }
            public string type { get; set; }
        }

        public class Parent
        {
            public string flightPlaceId { get; set; }
            public string displayCode { get; set; }
            public string name { get; set; }
            public string type { get; set; }
        }

        public class Price
        {
            public double raw { get; set; }
            public string formatted { get; set; }
            public string pricingOptionId { get; set; }
        }

        public class Segment
        {
            public string id { get; set; }
            public Origin origin { get; set; }
            public Destination destination { get; set; }
            public DateTime departure { get; set; }
            public DateTime arrival { get; set; }
            public int durationInMinutes { get; set; }
            public string flightNumber { get; set; }
            public MarketingCarrier marketingCarrier { get; set; }
            public OperatingCarrier operatingCarrier { get; set; }
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
            public string formattedPrice { get; set; }
        }


    }
}
