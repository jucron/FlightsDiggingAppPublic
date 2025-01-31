using static FlightsDiggingApp.Models.GetAirportsResponse;

namespace FlightsDiggingApp.Models
{
    public class GetRoundtripsResponse
    {
        public string from { get; set; }
        public string to { get; set; }
        public string currency { get; set; }
        public string initDepartDateString { get; set; }
        public string endDepartDateString { get; set; }
        public string initReturnDateString { get; set; }
        public string endReturnDateString { get; set; }
    }
}
