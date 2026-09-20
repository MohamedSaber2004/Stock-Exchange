using Microsoft.AspNetCore.Identity;

namespace Stock_Exchange.Application.Common.Models
{
    public class Result
    {
        public bool Succeeded { get; set; }
        public object? MainResult { get; set; }
        public object? Data { get; set; }
        public string[] Errors { get; set; }

        internal Result(bool succeeded, object? mainResult, object? data, IEnumerable<string> errors)
        {
            MainResult = mainResult;
            Succeeded = succeeded;
            Errors = errors.ToArray();
            Data = data;
        }
    }

    public static class IdentityResultMap
    {

        public static Result Success(this IdentityResult mainResult, object data)
        {
            return new Result(true, mainResult, data, System.Array.Empty<string>());
        }

        public static Result Success(this IdentityResult mainResult)
        {
            return new Result(true, mainResult, null, System.Array.Empty<string>());
        }

        public static Result Failure(this IdentityResult mainResult, IEnumerable<string> errors)
        {
            return new Result(false, mainResult, null, errors);
        }
    }

    public static class SginInResultMap
    {

        public static Result Success(this SignInResult mainResult, object data)
        {
            return new Result(true, mainResult, data, System.Array.Empty<string>());
        }

        public static Result Success(this SignInResult mainResult)
        {
            return new Result(true, mainResult, null, System.Array.Empty<string>());
        }

        public static Result Failure(this SignInResult mainResult, IEnumerable<string> errors)
        {
            return new Result(false, mainResult, null, errors);
        }
    }

    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public bool IsSuccess => Succeeded;
        public int StatusCode { get; set; } = 200;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();

        public Result() { }

        public Result(bool succeeded, int statusCode, string message, T? data, string[]? errors = null)
        {
            Succeeded = succeeded;
            StatusCode = statusCode;
            Message = message;
            Data = data;
            Errors = errors ?? Array.Empty<string>();
        }

        public static Result<T> Success(T data)
            => new(true, 200, string.Empty, data);

        public static Result<T> Success(T data, string message, int statusCode = 200)
            => new(true, statusCode, message, data);

        public static Result<T> Failure(string message, int statusCode = 400, params string[] errors)
            => new(false, statusCode, message, default, errors.Length > 0 ? errors : new[] { message });

        public static Result<T> Failure(params string[] errors)
            => new(false, 400, errors.FirstOrDefault() ?? string.Empty, default, errors);

        public ApiResponse<T> ToApiResponse()
        {
            if (Succeeded)
                return ApiResponse<T>.Ok(Data, Message, StatusCode);

            var dict = new Dictionary<string, string[]>();
            if (Errors.Length > 0)
                dict["General"] = Errors;
            else if (!string.IsNullOrWhiteSpace(Message))
                dict["General"] = new[] { Message };

            return ApiResponse<T>.Error(dict, Message, StatusCode);
        }
    }

    public static class ResultExtensions
    {
        public static Result<T> ToResult<T>(this T data) => Result<T>.Success(data);
    }
}
