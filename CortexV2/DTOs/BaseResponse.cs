using System.Text.Json.Serialization;

namespace Cortex.DTOs
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PaginationMeta? Pagination { get; set; }

        // Success constructors
        public static BaseResponse<T> Ok(T data, string message = "Operation successful")
        {
            return new BaseResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static BaseResponse<T> Created(T data, string message = "Resource created successfully")
        {
            return new BaseResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        // Failure constructors
        public static BaseResponse<T> Fail(string message, List<string>? errors = null)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }

        public static BaseResponse<T> NotFound(string message = "Resource not found")
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message
            };
        }

        public static BaseResponse<T> ValidationError(List<string> errors, string message = "Validation failed")
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }

    public class PaginationMeta
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}

