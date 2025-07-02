using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Helpers
{
    public class ApiCallResponse<T>
    {
        public OperationStatus operationStatus { get; set; }
        public T? data { get; set; }
    }
}
