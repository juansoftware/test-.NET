using System.Net; // Import System.Net for HTTP status codes

namespace StargateAPI.Controllers // Define namespace for Stargate API controllers
{
    public class BaseResponse // Base response class for API responses
    {
        public bool Success { get; set; } = true; // Property indicating if operation was successful, defaults to true
        public string Message { get; set; } = "Successful"; // Property containing response message, defaults to "Successful"
        public int ResponseCode { get; set; } = (int)HttpStatusCode.OK; // Property containing HTTP status code, defaults to 200 OK
    }
}