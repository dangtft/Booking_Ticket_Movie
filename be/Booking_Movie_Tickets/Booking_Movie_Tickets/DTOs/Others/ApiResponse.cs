namespace Booking_Movie_Tickets.DTOs.Others
{
    public class ApiResponse<T> : PagedResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string>? Errors { get; set; }

        public ApiResponse(T data, string message = ApiMessages.SUCCESS)
        {
            Success = true;
            Message = message;
            Errors = null;
            Data = new List<T> { data };
        }

        // Constructor cho lỗi
        public ApiResponse(string message, List<string>? errors = null)
        {
            Success = false;
            Message = message;
            Errors = errors ?? new List<string>();
            Data = new List<T>();
        }

        // Constructor cho dữ liệu có phân trang
        public ApiResponse(PagedResult<T> pagedData, string message = ApiMessages.SUCCESS)
        {
            Success = true;
            Message = message;
            Errors = null;
            Page = pagedData.Page;
            PageSize = pagedData.PageSize;
            TotalCount = pagedData.TotalCount;
            TotalPages = pagedData.TotalPages;
            Data = pagedData.Data;
        }

        // Success Response cho dữ liệu thường
        public static ApiResponse<T> SuccessResponse(T data, string message = ApiMessages.SUCCESS)
        {
            return new ApiResponse<T>(data, message);
        }

        // Success Response cho dữ liệu có phân trang
        public static ApiResponse<T> SuccessPagedResponse(PagedResult<T> pagedData, string message = ApiMessages.SUCCESS)
        {
            return new ApiResponse<T>(pagedData, message);
        }

        // Error Response
        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>(message, errors);
        }
    }
}
