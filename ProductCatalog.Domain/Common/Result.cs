
namespace ProductCatalog.Domain.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public string? ErrorMessage { get; }
        public List<string>? ValidationErrors { get; }
        public int StatusCode { get; }

        private Result(bool isSuccess, T? data, string? errorMessage,
                      List<string>? validationErrors, int statusCode)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            ValidationErrors = validationErrors;
            StatusCode = statusCode;
        }

        // Success factories
        public static Result<T> Success(T data)
            => new Result<T>(true, data, null, null, 200);

        public static Result<T> Success(T data, int statusCode)
            => new Result<T>(true, data, null, null, statusCode);

        public static Result<T> Created(T data)
            => new Result<T>(true, data, null, null, 201);

        // Failure factories
        public static Result<T> Failure(string errorMessage, int statusCode = 400)
            => new Result<T>(false, default, errorMessage, null, statusCode);

        public static Result<T> ValidationFailure(List<string> validationErrors)
            => new Result<T>(false, default, "Validation failed", validationErrors, 400);


        public static Result<T> NotFound(string message = "Resource not found")
            => new Result<T>(false, default, message, null, 404);

        public static Result<T> Unauthorized(string message = "Unauthorized")
            => new Result<T>(false, default, message, null, 401);

        public static Result<T> Forbidden(string message = "Forbidden")
            => new Result<T>(false, default, message, null, 403);

        // Helper methods
        public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
            => IsSuccess ? Result<TResult>.Success(mapper(Data!)) : Result<TResult>.Failure(ErrorMessage!, StatusCode);

        public T GetValueOrThrow() => IsSuccess ? Data! : throw new InvalidOperationException(ErrorMessage);
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        public List<string>? ValidationErrors { get; }
        public int StatusCode { get; }

        private Result(bool isSuccess, string? errorMessage,
                      List<string>? validationErrors, int statusCode)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ValidationErrors = validationErrors;
            StatusCode = statusCode;
        }

        public static Result Success() => new Result(true, null, null, 200);
        public static Result Success(int statusCode) => new Result(true, null, null, statusCode);
        public static Result Failure(string errorMessage, int statusCode = 400)
            => new Result(false, errorMessage, null, statusCode);
        public static Result ValidationFailure(List<string> validationErrors)
            => new Result(false, "Validation failed", validationErrors, 400);
    }
}
