using MediatR; // Import MediatR for mediator pattern implementation
using MediatR.Pipeline; // Import MediatR pipeline for pre/post processing
using Microsoft.EntityFrameworkCore; // Import Entity Framework Core
using StargateAPI.Business.Data; // Import data models
using StargateAPI.Controllers; // Import controller base classes

namespace StargateAPI.Business.Commands // Define namespace for business commands
{
    public class UpdatePerson : IRequest<UpdatePersonResult> // Command class for updating person
    {
        public required string Name { get; set; } = string.Empty; // Required property for current person name with default empty string
        public string? NewName { get; set; } // Optional property for new person name
    }

    public class UpdatePersonPreProcessor : IRequestPreProcessor<UpdatePerson> // Pre-processor for validation before command execution
    {
        private readonly StargateContext _context; // Private field for database context
        public UpdatePersonPreProcessor(StargateContext context) // Constructor to inject database context
        {
            _context = context; // Assign injected context to private field
        }
        
        public Task Process(UpdatePerson request, CancellationToken cancellationToken) // Method to process validation
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Name)) // Check if current name is null or whitespace
                throw new ArgumentException("Person name cannot be null or empty.", nameof(request.Name)); // Throw exception for invalid current name

            if (string.IsNullOrWhiteSpace(request.NewName)) // Check if new name is null or whitespace
                throw new ArgumentException("New name cannot be null or empty.", nameof(request.NewName)); // Throw exception for invalid new name

            // Check if person exists
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name); // Find person by current name without tracking
            if (person is null) // Check if person exists
                throw new InvalidOperationException($"Person with name '{request.Name}' not found."); // Throw exception if person not found

            // Check if new name already exists (if changing name)
            if (request.Name != request.NewName) // Check if name is actually changing
            {
                var existingPerson = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.NewName); // Find person by new name without tracking
                if (existingPerson is not null) // Check if person with new name already exists
                    throw new InvalidOperationException($"Person with name '{request.NewName}' already exists."); // Throw exception for duplicate name
            }

            return Task.CompletedTask; // Return completed task
        }
    }

    public class UpdatePersonHandler : IRequestHandler<UpdatePerson, UpdatePersonResult> // Handler for updating person
    {
        private readonly StargateContext _context; // Private field for database context

        public UpdatePersonHandler(StargateContext context) // Constructor to inject database context
        {
            _context = context; // Assign injected context to private field
        }
        
        public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken) // Method to handle command execution
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Name == request.Name, cancellationToken); // Find person by name with tracking
            
            if (person == null) // Check if person was found
            {
                throw new InvalidOperationException($"Person with name '{request.Name}' not found."); // Throw exception if person not found
            }

            person.Name = request.NewName!; // Update person name to new name
            _context.People.Update(person); // Mark person as updated in context
            await _context.SaveChangesAsync(cancellationToken); // Save changes to database

            return new UpdatePersonResult() // Return successful result
            {
                Id = person.Id, // Set ID of updated person
                Message = $"Person '{request.Name}' updated to '{request.NewName}' successfully." // Set success message
            };
        }
    }

    public class UpdatePersonResult : BaseResponse // Result class for update person operation
    {
        public int Id { get; set; } // Property to hold the ID of updated person
    }
}
