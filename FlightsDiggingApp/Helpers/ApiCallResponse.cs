using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Helpers
{
    public class ApiCallResponse<T>
    {
        public OperationStatus status { get; set; }
        public T? data { get; set; }
    }
}
