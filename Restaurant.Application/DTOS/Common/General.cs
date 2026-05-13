using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Common
{
    // Pagination
    public class PaginationDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int Skip => (PageNumber - 1) * PageSize;
    }

    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    // Result Pattern for Service Layer
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public string? Message { get; }
        public T? Value { get; }
        public List<string>? Errors { get; }

        protected Result(bool isSuccess, T? value, string? message, List<string>? errors)
        {
            IsSuccess = isSuccess;
            Value = value;
            Message = message;
            Errors = errors;
        }

        public static Result<T> Success(T value, string? message = null) => new(true, value, message, null);
        public static Result<T> Failure(string message, List<string>? errors = null) => new(false, default, message, errors);
    }

    // Explicit generic version for void results
    public class Result : Result<object>
    {
        private Result(bool isSuccess, string? message, List<string>? errors) : base(isSuccess, null, message, errors) { }
        public static Result Success(string? message = null) => new Result(true, message, null);
        public static new Result Failure(string message, List<string>? errors = null) => new Result(false, message, errors);
    }

    // API Response Wrapper
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponseDto<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponseDto<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponseDto<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponseDto<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }

    // Base Entity DTO
    public abstract class BaseEntityDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
