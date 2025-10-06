using StargateAPI.Business.Data; // Import data models

namespace StargateAPI.Business.Services // Define namespace for business services
{
    public interface IAuditService // Interface defining audit service contract
    {
        Task LogSuccessAsync(string action, string entityType, string entityId, string details = "", string userId = ""); // Method signature for logging successful operations
        Task LogErrorAsync(string action, string entityType, string entityId, Exception exception, string userId = ""); // Method signature for logging error operations
        Task LogInformationAsync(string action, string message, string userId = ""); // Method signature for logging general information
    }
}
