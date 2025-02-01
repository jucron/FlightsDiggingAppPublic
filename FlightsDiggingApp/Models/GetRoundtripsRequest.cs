using static FlightsDiggingApp.Models.GetAirportsResponse;

namespace FlightsDiggingApp.Models
{
    public record GetRoundtripsRequest
    {
        public string from;
        public string to;
        public string currency;
        public int adults;
        public int childs;
        public string initDepartDateString;
        public string endDepartDateString;
        public string initReturnDateString;
        public string endReturnDateString;
        public string departDate;
        public string returnDate;
        public string sessionId;
    }
}
