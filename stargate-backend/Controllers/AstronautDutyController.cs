using MediatR; // Import MediatR for mediator pattern implementation
using Microsoft.AspNetCore.Mvc; // Import ASP.NET Core MVC framework
using StargateAPI.Business.Commands; // Import business commands for astronaut duty operations
using StargateAPI.Business.Queries; // Import business queries for astronaut duty operations
using System.Net; // Import System.Net for HTTP status codes

namespace StargateAPI.Controllers // Define namespace for Stargate API controllers
{
    [ApiController] // Mark this class as an API controller
    [Route("[controller]")] // Define route template using controller name
    public class AstronautDutyController : ControllerBase // Controller for managing astronaut duties
    {
        private readonly IMediator _mediator; // Private field to hold MediatR mediator instance
        public AstronautDutyController(IMediator mediator) // Constructor to inject MediatR mediator
        {
            _mediator = mediator; // Assign injected mediator to private field
        }

        [HttpGet("{name}")] // HTTP GET endpoint with name parameter
        public async Task<IActionResult> GetAstronautDutiesByName(string name) // Method to retrieve astronaut duties by person name
        {
            try // Begin try block for exception handling
            {
                if (string.IsNullOrWhiteSpace(name)) // Check if name parameter is null or whitespace
                {
                    return this.GetResponse(new BaseResponse() // Return error response for invalid name
                    {
                        Message = "Name cannot be null or empty.", // Error message for invalid name
                        Success = false, // Set success flag to false
                        ResponseCode = (int)HttpStatusCode.BadRequest // Set HTTP status code to 400
                    });
                }

                var result = await _mediator.Send(new GetAstronautDutiesByName() // Send query to mediator for processing
                {
                    Name = name // Set name property in query object
                });

                return this.GetResponse(result); // Return successful response with query result
            }
            catch (Exception ex) // Catch any exceptions that occur
            {
                return this.GetResponse(new BaseResponse() // Return error response for exceptions
                {
                    Message = ex.Message, // Set error message from exception
                    Success = false, // Set success flag to false
                    ResponseCode = (int)HttpStatusCode.InternalServerError // Set HTTP status code to 500
                });
            }            
        }

        [HttpPost("")] // HTTP POST endpoint for creating astronaut duty
        public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request) // Method to create new astronaut duty
        {
            try // Begin try block for exception handling
            {
                if (request == null) // Check if request body is null
                {
                    return this.GetResponse(new BaseResponse() // Return error response for null request
                    {
                        Message = "Request body cannot be null.", // Error message for null request
                        Success = false, // Set success flag to false
                        ResponseCode = (int)HttpStatusCode.BadRequest // Set HTTP status code to 400
                    });
                }

                var result = await _mediator.Send(request); // Send command to mediator for processing
                return this.GetResponse(result); // Return response with command result           
            }
            catch (Exception ex) // Catch any exceptions that occur
            {
                return this.GetResponse(new BaseResponse() // Return error response for exceptions
                {
                    Message = ex.Message, // Set error message from exception
                    Success = false, // Set success flag to false
                    ResponseCode = (int)HttpStatusCode.InternalServerError // Set HTTP status code to 500
                });
            }
        }
    }
}