using Payment.Dtos;

namespace Payment.Services.CreationExceptions.Fabric
{
    public static class RegitrationDI
    {
        public static void AddExceptionFabric(this IServiceCollection collection)
        {
            collection.AddKeyedScoped<ExceptionsFabric, IdempotanceKeyFabric>(
                ProblemsCodes.idempotence_key_required.ToString()
                );
            collection.AddKeyedScoped<ExceptionsFabric, IdempotenceConflictFabric>(
                ProblemsCodes.idempotence_key_conflict.ToString()
                );
            collection.AddKeyedScoped<ExceptionsFabric, NotFoundFabric>(
                ProblemsCodes.not_found.ToString()
                );
            collection.AddKeyedScoped<ExceptionsFabric, RefundAmountExceededFabric>(
                ProblemsCodes.refund_amount_exceeded.ToString()
                );
            collection.AddKeyedScoped<ExceptionsFabric, RefundNotAllowedFabric>(
                ProblemsCodes.refund_not_allowed.ToString()
                );
            collection.AddKeyedScoped<ExceptionsFabric, UnauthorizedFabric>(
                ProblemsCodes.unauthorized.ToString()
                );
            collection.AddKeyedScoped<ExceptionsFabric, ValidationFabric>(
                ProblemsCodes.validation_error.ToString()
                );
        }
    }
}
