namespace API.Errors
{
    public class ApiException
    {
        public ApiException(int statusCode, string msg=null, string details=null)
        {
            StatusCode = statusCode;
            this.msg = msg;
            this.details = details;
        }

        public int StatusCode { get; set; }
        public string msg { get; set; }
        public string details { get; set; }
    }
}