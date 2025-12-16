using ProductCatalog.Domain.Common;

namespace ProductCatalog.Domain.Exceptions
{
    public abstract class BaseException : Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }

        protected BaseException(string message, string errorCode, int statusCode)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        protected BaseException(string message, string errorCode, int statusCode, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : BaseException
    {
        public NotFoundException(string entityName, object id)
            : base($"{entityName} with ID {id} was not found.",
                  ErrorCodes.ProductNotFound, 404)
        {
        }
    }

    public class ValidationException : BaseException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("One or more validation errors occurred.",
                  ErrorCodes.ValidationError, 400)
        {
            Errors = errors;
        }
    }

    public class BusinessRuleException : BaseException
    {
        public BusinessRuleException(string message, string errorCode)
            : base(message, errorCode, 400)
        {
        }
    }

    public class DatabaseException : BaseException
    {
        public DatabaseException(Exception innerException)
            : base("A database error occurred.",
                  ErrorCodes.DatabaseError, 500, innerException)
        {
        }
    }
}
