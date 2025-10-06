using MediatR; // Import MediatR for mediator pattern implementation
using MediatR.Pipeline; // Import MediatR pipeline for pre/post processing
using Microsoft.EntityFrameworkCore; // Import Entity Framework Core
using StargateAPI.Business.Data; // Import data models
using StargateAPI.Business.Services; // Import business services
using StargateAPI.Controllers; // Import controller base classes
using Microsoft.AspNetCore.Http; // Import ASP.NET Core HTTP classes

namespace StargateAPI.Business.Commands // Define namespace for business commands
{
    public class CreatePerson : IRequest<CreatePersonResult> // Command class for creating person
    {
        public required string Name { get; set; } = string.Empty; // Required property for person name with default empty string
    }

    public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson> // Pre-processor for validation before command execution
    {
        private readonly StargateContext _context; // Private field for database context
        public CreatePersonPreProcessor(StargateContext context) // Constructor to inject database context
        {
            _context = context; // Assign injected context to private field
        }
        public Task Process(CreatePerson request, CancellationToken cancellationToken) // Method to process validation
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Name)) // Check if name is null or whitespace
                throw new BadHttpRequestException("Bad Request"); // Throw exception for invalid name

            // Check for duplicate names
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name); // Find person by name without tracking

            if (person is not null) throw new BadHttpRequestException("Bad Request"); // Throw exception if person already exists

            return Task.CompletedTask; // Return completed task
        }
    }

    public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult> // Handler for creating person
    {
        private readonly StargateContext _context; // Private field for database context
        private readonly IAuditService _auditService; // Private field for audit service

        public CreatePersonHandler(StargateContext context, IAuditService auditService) // Constructor to inject dependencies
        {
            _context = context; // Assign injected context to private field
            _auditService = auditService; // Assign injected audit service to private field
        }
        public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken) // Method to handle command execution
        {
            try // Begin try block for exception handling
            {
                var newPerson = new Person() // Create new person instance
                {
                   Name = request.Name // Set person name
                };

                await _context.People.AddAsync(newPerson); // Add new person to context
                await _context.SaveChangesAsync(); // Save changes to database

                await _auditService.LogSuccessAsync("CREATE_PERSON", "Person", newPerson.Id.ToString(), $"Created person: {request.Name}", "System"); // Log successful operation

                return new CreatePersonResult() // Return successful result
                {
                    Id = newPerson.Id, // Set ID of created person
                    Message = $"Person '{request.Name}' created successfully." // Set success message
                };
            }
            catch (Exception ex) // Catch any exceptions
            {
                await _auditService.LogErrorAsync("CREATE_PERSON", "Person", request.Name, ex, "System"); // Log error
                throw; // Re-throw exception
            }
        }
    }

    public class CreatePersonResult : BaseResponse // Result class for create person operation
    {
        public int Id { get; set; } // Property to hold the ID of created person
    }
}
