using Microsoft.EntityFrameworkCore; // Import Entity Framework Core
using StargateAPI.Business.Data; // Import data models

namespace StargateAPI.Business.Services // Define namespace for business services
{
    public class AuditService : IAuditService // Service class for audit logging functionality
    {
        private readonly StargateContext _context; // Private field for database context
        private readonly ILogger<AuditService> _logger; // Private field for logging service

        public AuditService(StargateContext context, ILogger<AuditService> logger) // Constructor to inject dependencies
        {
            _context = context; // Assign injected context to private field
            _logger = logger; // Assign injected logger to private field
        }

        public async Task LogSuccessAsync(string action, string entityType, string entityId, string details = "", string userId = "") // Method to log successful operations
        {
            var auditLog = new AuditLog // Create new audit log entry
            {
                Action = action, // Set action performed
                EntityType = entityType, // Set type of entity affected
                EntityId = entityId, // Set ID of entity affected
                Details = details, // Set additional details
                UserId = userId, // Set user ID who performed action
                Level = "Information", // Set log level to Information
                Message = $"Success: {action} for {entityType} {entityId}" // Set success message
            };

            await _context.AuditLogs.AddAsync(auditLog); // Add audit log to context
            await _context.SaveChangesAsync(); // Save changes to database
            
            _logger.LogInformation("Audit: {Action} for {EntityType} {EntityId} - {Details}", action, entityType, entityId, details); // Log to application logger
        }

        public async Task LogErrorAsync(string action, string entityType, string entityId, Exception exception, string userId = "") // Method to log error operations
        {
            var auditLog = new AuditLog // Create new audit log entry
            {
                Action = action, // Set action that failed
                EntityType = entityType, // Set type of entity affected
                EntityId = entityId, // Set ID of entity affected
                Details = exception.ToString(), // Set exception details
                UserId = userId, // Set user ID who performed action
                Level = "Error", // Set log level to Error
                Message = $"Error: {action} for {entityType} {entityId} - {exception.Message}" // Set error message
            };

            await _context.AuditLogs.AddAsync(auditLog); // Add audit log to context
            await _context.SaveChangesAsync(); // Save changes to database
            
            _logger.LogError(exception, "Audit Error: {Action} for {EntityType} {EntityId}", action, entityType, entityId); // Log error to application logger
        }

        public async Task LogInformationAsync(string action, string message, string userId = "") // Method to log general information
        {
            var auditLog = new AuditLog // Create new audit log entry
            {
                Action = action, // Set action performed
                EntityType = "System", // Set entity type to System
                EntityId = "", // Set empty entity ID
                Details = message, // Set message details
                UserId = userId, // Set user ID
                Level = "Information", // Set log level to Information
                Message = $"Info: {action} - {message}" // Set information message
            };

            await _context.AuditLogs.AddAsync(auditLog); // Add audit log to context
            await _context.SaveChangesAsync(); // Save changes to database
            
            _logger.LogInformation("Audit Info: {Action} - {Message}", action, message); // Log to application logger
        }
    }
}
