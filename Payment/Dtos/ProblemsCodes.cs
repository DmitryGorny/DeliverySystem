namespace Payment.Dtos
{
    public enum ProblemsCodes
    {
        validation_error,
        idempotence_key_required,
        unauthorized,
        not_found,
        idempotence_key_conflict,
        refund_not_allowed,
        refund_amount_exceeded
    }
}
