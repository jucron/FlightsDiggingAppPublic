namespace FlightsDiggingApp.Models
{
    public class OperationStatus
    {
        public string Status { get; set; }
        public bool hasError { set; get; }
        public string errorDescription { set; get; }

        public static OperationStatus CreateStatusSuccess()
        {
            return new OperationStatus
            {
                Status = "Success",
                hasError = false,
                errorDescription = ""
            };
        }
        public static OperationStatus CreateStatusFailure(string description) {
            return new OperationStatus
            {
                Status = "Failure",
                hasError = true,
                errorDescription = description
            };
        }
    }
}
