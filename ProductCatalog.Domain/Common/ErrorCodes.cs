
namespace ProductCatalog.Domain.Common
{
    public static class ErrorCodes
    {
        // Product errors
        public const string ProductNotFound = "PRODUCT_NOT_FOUND";
        public const string ProductNameDuplicate = "PRODUCT_NAME_DUPLICATE";
        public const string ProductPriceInvalid = "PRODUCT_PRICE_INVALID";
        public const string ProductQuantityInvalid = "PRODUCT_QUANTITY_INVALID";

        // System errors
        public const string DatabaseError = "DATABASE_ERROR";
        public const string ValidationError = "VALIDATION_ERROR";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    }
}
