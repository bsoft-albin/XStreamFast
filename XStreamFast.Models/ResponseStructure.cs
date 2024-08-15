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
}
