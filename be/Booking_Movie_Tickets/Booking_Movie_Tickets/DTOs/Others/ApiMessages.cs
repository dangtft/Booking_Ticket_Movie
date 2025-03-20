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

        // Pagination Messages
        public const string INVALID_PAGINATION = "Invalid pagination parameters.";
    }
}
