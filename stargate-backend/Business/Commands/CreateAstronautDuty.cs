using Dapper; // Import Dapper for micro-ORM functionality
using MediatR; // Import MediatR for mediator pattern implementation
using MediatR.Pipeline; // Import MediatR pipeline for pre/post processing
using Microsoft.EntityFrameworkCore; // Import Entity Framework Core
using StargateAPI.Business.Data; // Import data models
using StargateAPI.Business.Services; // Import business services
using StargateAPI.Controllers; // Import controller base classes
using System.Net; // Import System.Net for HTTP status codes

namespace StargateAPI.Business.Commands // Define namespace for business commands
{
    public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult> // Command class for creating astronaut duty
    {
        public required string Name { get; set; } // Required property for person name

        public required string Rank { get; set; } // Required property for astronaut rank

        public required string DutyTitle { get; set; } // Required property for duty title

        public DateTime DutyStartDate { get; set; } // Property for duty start date
    }

    public class CreateAstronautDutyPreProcessor : IRequestPreProcessor<CreateAstronautDuty> // Pre-processor for validation before command execution
    {
        private readonly StargateContext _context; // Private field for database context

        public CreateAstronautDutyPreProcessor(StargateContext context) // Constructor to inject database context
        {
            _context = context; // Assign injected context to private field
        }

        public Task Process(CreateAstronautDuty request, CancellationToken cancellationToken) // Method to process validation
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Name)) // Check if name is null or whitespace
                throw new ArgumentException("Person name cannot be null or empty.", nameof(request.Name)); // Throw exception for invalid name
            
            if (string.IsNullOrWhiteSpace(request.Rank)) // Check if rank is null or whitespace
                throw new ArgumentException("Rank cannot be null or empty.", nameof(request.Rank)); // Throw exception for invalid rank
                
            if (string.IsNullOrWhiteSpace(request.DutyTitle)) // Check if duty title is null or whitespace
                throw new ArgumentException("Duty title cannot be null or empty.", nameof(request.DutyTitle)); // Throw exception for invalid duty title

            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name); // Find person by name without tracking

            if (person is null)  // Check if person exists
                throw new InvalidOperationException($"Person with name '{request.Name}' not found."); // Throw exception if person not found

            // Check if person already has a current duty (no end date)
            var currentDuty = _context.AstronautDuties.FirstOrDefault(z => z.PersonId == person.Id && z.DutyEndDate == null); // Find current active duty

            if (currentDuty is not null && currentDuty.DutyTitle != "RETIRED")  // Check if person has active duty that's not retired
                throw new InvalidOperationException($"Person '{request.Name}' already has an active duty '{currentDuty.DutyTitle}'. Cannot assign new duty without ending current one."); // Throw exception for active duty conflict

            return Task.CompletedTask; // Return completed task
        }
    }

    public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult> // Handler for creating astronaut duty
    {
        private readonly StargateContext _context; // Private field for database context
        private readonly IAuditService _auditService; // Private field for audit service

        public CreateAstronautDutyHandler(StargateContext context, IAuditService auditService) // Constructor to inject dependencies
        {
            _context = context; // Assign injected context to private field
            _auditService = auditService; // Assign injected audit service to private field
        }
        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken) // Method to handle command execution
        {
            try // Begin try block for exception handling
            {
            // Fix SQL injection by using parameterized queries
            var personQuery = "SELECT * FROM [Person] WHERE Name = @Name"; // SQL query to find person by name
            var person = await _context.Connection.QueryFirstOrDefaultAsync<Person>(personQuery, new { Name = request.Name }); // Execute query with parameter

            if (person == null) // Check if person was found
            {
                throw new InvalidOperationException($"Person with name '{request.Name}' not found."); // Throw exception if person not found
            }

            var astronautDetailQuery = "SELECT * FROM [AstronautDetail] WHERE PersonId = @PersonId"; // SQL query to find astronaut detail by person ID
            var astronautDetail = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDetail>(astronautDetailQuery, new { PersonId = person.Id }); // Execute query with parameter

            if (astronautDetail == null) // Check if astronaut detail exists
            {
                astronautDetail = new AstronautDetail(); // Create new astronaut detail instance
                astronautDetail.PersonId = person.Id; // Set person ID
                astronautDetail.CurrentDutyTitle = request.DutyTitle; // Set current duty title
                astronautDetail.CurrentRank = request.Rank; // Set current rank
                astronautDetail.CareerStartDate = request.DutyStartDate.Date; // Set career start date
                
                // Rule: Career End Date is one day before the Retired Duty Start Date
                if (request.DutyTitle == "RETIRED") // Check if duty title is RETIRED
                {
                    astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date; // Set career end date to day before retirement
                }

                await _context.AstronautDetails.AddAsync(astronautDetail); // Add new astronaut detail to context
            }
            else // If astronaut detail exists
            {
                astronautDetail.CurrentDutyTitle = request.DutyTitle; // Update current duty title
                astronautDetail.CurrentRank = request.Rank; // Update current rank
                
                // Rule: Career End Date is one day before the Retired Duty Start Date
                if (request.DutyTitle == "RETIRED") // Check if duty title is RETIRED
                {
                    astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date; // Set career end date to day before retirement
                }
                
                _context.AstronautDetails.Update(astronautDetail); // Update existing astronaut detail
            }

            // Rule: Person's Previous Duty End Date is set to the day before the New Astronaut Duty Start Date
            var currentDutyQuery = "SELECT * FROM [AstronautDuty] WHERE PersonId = @PersonId AND DutyEndDate IS NULL"; // SQL query to find current active duty
            var currentDuty = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDuty>(currentDutyQuery, new { PersonId = person.Id }); // Execute query with parameter

            if (currentDuty != null) // Check if current duty exists
            {
                currentDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date; // Set duty end date to day before new duty start
                _context.AstronautDuties.Update(currentDuty); // Update current duty
            }

            var newAstronautDuty = new AstronautDuty() // Create new astronaut duty instance
            {
                PersonId = person.Id, // Set person ID
                Rank = request.Rank, // Set rank
                DutyTitle = request.DutyTitle, // Set duty title
                DutyStartDate = request.DutyStartDate.Date, // Set duty start date
                DutyEndDate = null // Set duty end date to null (active duty)
            };

            await _context.AstronautDuties.AddAsync(newAstronautDuty); // Add new astronaut duty to context

            await _context.SaveChangesAsync(); // Save all changes to database

            await _auditService.LogSuccessAsync("CREATE_ASTRONAUT_DUTY", "AstronautDuty", newAstronautDuty.Id.ToString(),  // Log successful operation
                $"Created duty: {request.DutyTitle} for {request.Name}", "System"); // Log details

            return new CreateAstronautDutyResult() // Return successful result
            {
                Id = newAstronautDuty.Id // Set ID of created duty
            };
            }
            catch (Exception ex) // Catch any exceptions
            {
                await _auditService.LogErrorAsync("CREATE_ASTRONAUT_DUTY", "AstronautDuty", request.Name, ex, "System"); // Log error
                throw; // Re-throw exception
            }
        }
    }

    public class CreateAstronautDutyResult : BaseResponse // Result class for create astronaut duty operation
    {
        public int? Id { get; set; } // Property to hold the ID of created duty
    }
}
