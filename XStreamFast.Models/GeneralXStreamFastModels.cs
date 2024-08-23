namespace XStreamFast.Models
{
    public class BaseResponseModel <X> where X : new()
    {
        public X Data { get; set; } = new X();
        public Int32 StatusCode { get; set; } = 200;
        public String StatusMessage { get; set; } = "Success";
    }
    public class ErrorResponse
    {
        public int StatusCode { get; set; } = 500;
        public string Message { get; set; } = "";
        public string Error { get; set; } = ""; // For more detailed error information
    }

    public class RequestLog
    {
        public string HttpMethod { get; set; } = "";
        public string Endpoint { get; set; } = "";
        public Dictionary<string, string>? Headers { get; set; }
        public object? Body { get; set; }
    }

    public class ResponseLog
    {
        public string HttpMethod { get; set; } = "";
        public string Endpoint { get; set; } = "";
        public int StatusCode { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public object? Body { get; set; }
    }

    public class StringRequestLog
    {
        public string ClientIpAddress { get; set; } = "";
        public string CorrelationId { get; set; } = "";
        public string UserAgent { get; set; } = "";
        public string UserIdentity { get; set; } = "";
        public string QueryParameters { get; set; } = "";
        public string HttpMethod { get; set; } = "";
        public string Endpoint { get; set; } = "";
        public string Headers { get; set; } = "";
        public string Body { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class StringResponseLog
    {
        public string HttpMethod { get; set; } = "";
        public string Endpoint { get; set; } = "";
        public string Headers { get; set; } = "";
        public string Body { get; set; } = "";
        public string TotalTimeTaken { get; set; } = "";
        public int StatusCode { get; set; }
        public string Message { get; set; } = "";
    }
}
