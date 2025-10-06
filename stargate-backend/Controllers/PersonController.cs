using MediatR; // Import MediatR for mediator pattern implementation
using Microsoft.AspNetCore.Mvc; // Import ASP.NET Core MVC framework
using StargateAPI.Business.Commands; // Import business commands for person operations
using StargateAPI.Business.Queries; // Import business queries for person operations
using System.Net; // Import System.Net for HTTP status codes

namespace StargateAPI.Controllers // Define namespace for Stargate API controllers
{
   
    [ApiController] // Mark this class as an API controller
    [Route("[controller]")] // Define route template using controller name
    public class PersonController : ControllerBase // Controller for managing persons
    {
        private readonly IMediator _mediator; // Private field to hold MediatR mediator instance
        public PersonController(IMediator mediator) // Constructor to inject MediatR mediator
        {
            _mediator = mediator; // Assign injected mediator to private field
        }

        [HttpGet("")] // HTTP GET endpoint for retrieving all people
        public async Task<IActionResult> GetPeople() // Method to retrieve all people
        {
            try // Begin try block for exception handling
            {
                var result = await _mediator.Send(new GetPeople() // Send query to mediator for processing
                {

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

        [HttpGet("{name}")] // HTTP GET endpoint with name parameter
        public async Task<IActionResult> GetPersonByName(string name) // Method to retrieve person by name
        {
            try // Begin try block for exception handling
            {
                var result = await _mediator.Send(new GetPersonByName() // Send query to mediator for processing
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

        [HttpPost("")] // HTTP POST endpoint for creating person
        public async Task<IActionResult> CreatePerson([FromBody] string name) // Method to create new person
        {
            try // Begin try block for exception handling
            {
                if (string.IsNullOrWhiteSpace(name)) // Check if name parameter is null or whitespace
                {
                    return this.GetResponse(new BaseResponse() // Return error response for invalid name
                    {
                        Message = "Person name cannot be null or empty.", // Error message for invalid name
                        Success = false, // Set success flag to false
                        ResponseCode = (int)HttpStatusCode.BadRequest // Set HTTP status code to 400
                    });
                }

                var result = await _mediator.Send(new CreatePerson() // Send command to mediator for processing
                {
                    Name = name // Set name property in command object
                });

                return this.GetResponse(result); // Return successful response with command result
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

        [HttpPut("{name}")] // HTTP PUT endpoint with name parameter for updating person
        public async Task<IActionResult> UpdatePerson(string name, [FromBody] UpdatePersonRequest request) // Method to update existing person
        {
            try // Begin try block for exception handling
            {
                var result = await _mediator.Send(new UpdatePerson() // Send command to mediator for processing
                {
                    Name = name, // Set current name property in command object
                    NewName = request.NewName // Set new name property in command object
                });

                return this.GetResponse(result); // Return successful response with command result
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

    public class UpdatePersonRequest // Request model for updating person
    {
        public string NewName { get; set; } = string.Empty; // Property to hold the new name value
    }
}