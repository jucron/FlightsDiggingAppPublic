using static FlightsDiggingApp.Models.GetAirportsResponse;

namespace FlightsDiggingApp.Models
{
    public record GetRoundtripsRequest
    {
        public string from { get; set; }
        public string to { get; set; }
        public string currency { get; set; }
        public int adults { get; set; }
        public int childs { get; set; }
        public string initDepartDateString { get; set; }
        public string endDepartDateString { get; set; }
        public string initReturnDateString { get; set; }
        public string endReturnDateString { get; set; }

        public string departDate { get; set; }
        public string returnDate { get; set; }
    }
}
