namespace TestTask_AdsPlatform.Result
{
    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string>? Details { get; set; }

        private ErrorResponse(string errorCode, string message, Dictionary<string, string>? details = null)
        {
            ErrorCode = errorCode;
            Message = message;
            Details = details ?? [];
        }
        public static ErrorResponse Create(string errorCode, string message, Dictionary<string, string>? details = null)
        {
            return new ErrorResponse(errorCode, message, details);
        }
    }
}
