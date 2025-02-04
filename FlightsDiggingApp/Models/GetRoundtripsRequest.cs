using static FlightsDiggingApp.Models.GetAirportsResponse;

namespace FlightsDiggingApp.Models
{
    public class GetRoundtripsRequest
    {
        public string from { get; set; }
        public string to { get; set; }
        public string currency { get; set; }
        public int adults { get; set; }
        public List<GetRoundtripsResponse.Child> children { get; set; }
        public int infants { get; set; }
        public string cabinclass { get; set; }
        public string initDepartDateString { get; set; }
        public string endDepartDateString { get; set; }
        public string initReturnDateString { get; set; }
        public string endReturnDateString { get; set; }
        public string departDate { get; set; }
        public string returnDate { get; set; }
        public string sessionId { get; set; }
        public Filter filter { get; set; }

    }
}
