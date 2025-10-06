using Dapper; // Import Dapper for micro-ORM functionality
using MediatR; // Import MediatR for mediator pattern implementation
using StargateAPI.Business.Data; // Import data models
using StargateAPI.Business.Dtos; // Import data transfer objects
using StargateAPI.Controllers; // Import controller base classes

namespace StargateAPI.Business.Queries // Define namespace for business queries
{
    public class GetPersonByName : IRequest<GetPersonByNameResult> // Query class for retrieving person by name
    {
        public required string Name { get; set; } = string.Empty; // Required property for person name with default empty string
    }

    public class GetPersonByNameHandler : IRequestHandler<GetPersonByName, GetPersonByNameResult> // Handler for retrieving person by name
    {
        private readonly StargateContext _context; // Private field for database context
        public GetPersonByNameHandler(StargateContext context) // Constructor to inject database context
        {
            _context = context; // Assign injected context to private field
        }

        public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken) // Method to handle query execution
        {
            var result = new GetPersonByNameResult(); // Create result instance

            // Fix SQL injection by using parameterized query
            var query = "SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id WHERE a.Name = @Name"; // SQL query to get person and astronaut details by name

            var person = await _context.Connection.QueryAsync<PersonAstronaut>(query, new { Name = request.Name }); // Execute query with parameter

            result.Person = person.FirstOrDefault(); // Set first person found in result (or null if none found)

            return result; // Return result
        }
    }

    public class GetPersonByNameResult : BaseResponse // Result class for get person by name operation
    {
        public PersonAstronaut? Person { get; set; } // Property to hold person information (nullable)
    }
}
