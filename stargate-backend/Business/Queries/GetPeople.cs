using Dapper; // Import Dapper for micro-ORM functionality
using MediatR; // Import MediatR for mediator pattern implementation
using StargateAPI.Business.Data; // Import data models
using StargateAPI.Business.Dtos; // Import data transfer objects
using StargateAPI.Controllers; // Import controller base classes

namespace StargateAPI.Business.Queries // Define namespace for business queries
{
    public class GetPeople : IRequest<GetPeopleResult> // Query class for retrieving all people
    {

    }

    public class GetPeopleHandler : IRequestHandler<GetPeople, GetPeopleResult> // Handler for retrieving all people
    {
        public readonly StargateContext _context; // Public readonly field for database context
        public GetPeopleHandler(StargateContext context) // Constructor to inject database context
        {
            _context = context; // Assign injected context to field
        }
        public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken) // Method to handle query execution
        {
            var result = new GetPeopleResult(); // Create result instance

            var query = $"SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id"; // SQL query to get all people and their astronaut details

            var people = await _context.Connection.QueryAsync<PersonAstronaut>(query); // Execute query

            result.People = people.ToList(); // Set people list in result

            return result; // Return result
        }
    }

    public class GetPeopleResult : BaseResponse // Result class for get people operation
    {
        public List<PersonAstronaut> People { get; set; } = new List<PersonAstronaut> { }; // Property to hold list of people

    }
}
