using Dapper; // Import Dapper for micro-ORM functionality
using MediatR; // Import MediatR for mediator pattern implementation
using StargateAPI.Business.Data; // Import data models
using StargateAPI.Business.Dtos; // Import data transfer objects
using StargateAPI.Controllers; // Import controller base classes

namespace StargateAPI.Business.Queries // Define namespace for business queries
{
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult> // Query class for retrieving astronaut duties by name
    {
        public string Name { get; set; } = string.Empty; // Property for person name with default empty string
    }

    public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult> // Handler for retrieving astronaut duties by name
    {
        private readonly StargateContext _context; // Private field for database context

        public GetAstronautDutiesByNameHandler(StargateContext context) // Constructor to inject database context
        {
            _context = context; // Assign injected context to private field
        }

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken) // Method to handle query execution
        {
            var result = new GetAstronautDutiesByNameResult(); // Create result instance

            // Fix SQL injection by using parameterized queries
            var personQuery = "SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id WHERE a.Name = @Name"; // SQL query to get person and astronaut details

            var person = await _context.Connection.QueryFirstOrDefaultAsync<PersonAstronaut>(personQuery, new { Name = request.Name }); // Execute query with parameter

            if (person == null) // Check if person was found
            {
                throw new InvalidOperationException($"Person with name '{request.Name}' not found."); // Throw exception if person not found
            }

            result.Person = person; // Set person in result

            var dutiesQuery = "SELECT * FROM [AstronautDuty] WHERE PersonId = @PersonId Order By DutyStartDate Desc"; // SQL query to get astronaut duties ordered by start date

            var duties = await _context.Connection.QueryAsync<AstronautDuty>(dutiesQuery, new { PersonId = person.PersonId }); // Execute query with parameter

            result.AstronautDuties = duties.ToList(); // Set duties list in result

            return result; // Return result
        }
    }

    public class GetAstronautDutiesByNameResult : BaseResponse // Result class for get astronaut duties by name operation
    {
        public PersonAstronaut Person { get; set; } // Property to hold person information
        public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>(); // Property to hold list of astronaut duties
    }
}
