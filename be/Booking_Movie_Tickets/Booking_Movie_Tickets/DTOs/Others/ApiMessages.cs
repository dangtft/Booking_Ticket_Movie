namespace Booking_Movie_Tickets.DTOs.Others
{
    public class ApiMessages
    {
        // Success Messages
        public const string SUCCESS = "Request processed successfully.";
        public const string CREATED_SUCCESS = "Created successfully.";
        public const string UPDATED_SUCCESS = "Updated successfully.";
        public const string DELETED_SUCCESS = "Deleted successfully.";

        // Error Messages
        public const string ERROR = "An error occurred while processing the request.";
        public const string NOT_FOUND = "Data not found.";
        public const string INVALID_REQUEST = "Invalid request data.";
        public const string UNAUTHORIZED = "You are not authorized to perform this action.";
        public const string FORBIDDEN = "Access denied.";
        public const string INTERNAL_SERVER_ERROR = "Internal server error. Please try again later.";
        public const string SERVICE_UNAVAILABLE = "Service is temporarily unavailable. Please try again later.";
        public const string TIMEOUT = "Request timeout. Please try again.";
        public const string UNSUPPORTED_MEDIA_TYPE = "Unsupported media type.";

        // Validation Errors
        public const string VALIDATION_FAILED = "Validation failed. Please check your input.";
        public const string REQUIRED_FIELD_MISSING = "A required field is missing.";
        public const string INVALID_FORMAT = "Invalid data format.";

        // Conflict Errors (409)
        public const string CONFLICT = "Conflict detected. The resource already exists.";
        public const string DUPLICATE_ENTRY = "Duplicate entry. This record already exists.";
        public const string RESOURCE_LOCKED = "Resource is locked. Cannot proceed.";

        // Rate Limit & Throttling
        public const string TOO_MANY_REQUESTS = "Too many requests. Please slow down.";
        public const string RATE_LIMIT_EXCEEDED = "Rate limit exceeded. Try again later.";

        // Dependency Errors
        public const string EXTERNAL_SERVICE_ERROR = "An error occurred with an external service.";
        public const string DATABASE_ERROR = "A database error occurred.";
        public const string NETWORK_ERROR = "A network error occurred. Please check your connection.";

        // Pagination Messages
        public const string INVALID_PAGINATION = "Invalid pagination parameters.";
        public const string NO_MORE_RESULTS = "No more results available.";

        // Authentication & Authorization
        public const string TOKEN_EXPIRED = "Your authentication token has expired.";
        public const string TOKEN_INVALID = "Invalid authentication token.";
        public const string ACCOUNT_SUSPENDED = "Your account has been suspended.";
        public const string PASSWORD_INCORRECT = "Incorrect password.";
    }
}
