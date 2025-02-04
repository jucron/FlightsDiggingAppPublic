using static FlightsDiggingApp.Models.AirportsResponse;

namespace FlightsDiggingApp.Models
{
    public class RoundtripsResponse
    {
        public string from { set; get; }
        public string to { set; get; }
        public string departDate{ set; get; }
        public string returnDate{ set; get; }
        public string currency{ set; get; }
        public string link{ set; get; }
        public int adults{ set; get; }
        public List<Child> children{ set; get; }
        public int infants{ set; get; }
        public string cabinclass{ set; get; }

        public List<Flight> flights{ set; get; }
        public OperationStatus status { set; get; }
        
        public record Flight
        {
            public double rawPrice { set; get; }
            public int hours { set; get; }
            public int stops { set; get; }
            public double score { set; get; }
            public List<Company> companies{ set; get; }

        }

        public record Company
        {
            public string logoUrl{ set; get; }
            public string name{ set; get; }
        }
        public record Child 
        {
            public int age{ set; get; }
        }
    }
    
}
/*
 * example for test: wss://localhost:7253/api/flightsdigger/getroundtrips
  {
  "from": "GIG",
  "to": "OPO",
  "currency": "BRL",
  "adults": 2,
  "children": [
    { "age": 3 },
    { "age": 7 }
  ],
  "infants": 0,
  "cabinclass": "economy",
  "initDepartDateString": "2025-06-06",
  "endDepartDateString": "2025-06-06",
  "initReturnDateString": "2025-06-25",
  "endReturnDateString": "2025-06-25",
  "departDate": "",
  "returnDate": "",
  "sessionId": ""
}
 * 
 */