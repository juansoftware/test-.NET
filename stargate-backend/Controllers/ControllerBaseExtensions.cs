using Microsoft.AspNetCore.Mvc; // Import ASP.NET Core MVC framework

namespace StargateAPI.Controllers // Define namespace for Stargate API controllers
{
    public static class ControllerBaseExtensions // Static class containing extension methods for ControllerBase
    {

        public static IActionResult GetResponse(this ControllerBase controllerBase, BaseResponse response) // Extension method to create standardized response
        {
            var httpResponse = new ObjectResult(response); // Create ObjectResult with BaseResponse
            httpResponse.StatusCode = response.ResponseCode; // Set HTTP status code from response
            return httpResponse; // Return the configured HTTP response
        }
    }
}