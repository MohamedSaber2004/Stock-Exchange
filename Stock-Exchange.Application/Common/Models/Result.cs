using Microsoft.AspNetCore.Http;

namespace Stock_Exchange.Application.Common.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public int StatusCode { get; }
        public string Message { get; }
        public T? Data { get; }
        public List<string> Errors { get; }

        public Result(bool isSuccess, int statusCode, string message, T? data, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
        }

        public static Result<T> Success(T? data, string message, int statusCode = StatusCodes.Status200OK)
            => new(true, statusCode, message, data);

        public static Result<T> Failure(string message, int statusCode, List<string>? errors = null)
            => new(false, statusCode, message, default, errors ?? new List<string> { message });

        public static Result<T> Failure(string message, int statusCode, string error)
            => new(false, statusCode, message, default, new List<string> { error });
    }
}
