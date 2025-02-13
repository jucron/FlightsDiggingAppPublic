using System.Net;

namespace FlightsDiggingApp.Models
{
    public class OperationStatus
    {
        public Status status { get; set; }
        public bool hasError { set; get; }
        public string errorDescription { set; get; }

        public class Status
        {
            public HttpStatusCode httpStatus { get; set; }
            public string description { get; set; }
        }
        public static OperationStatus CreateStatusSuccess(HttpStatusCode httpStatus)
        {
            return new OperationStatus
            {
                status = new Status { httpStatus = httpStatus, description = "Success" },
                hasError = false,
                errorDescription = ""
            };
        }
        public static OperationStatus CreateStatusFailure(HttpStatusCode httpStatus, string description) {
            return new OperationStatus
            {
                status = new Status { httpStatus = httpStatus, description = "Failure" },
                hasError = true,
                errorDescription = description
            };
        }
    }
}
