namespace FakeBank.Infrastructure
{
    // Ошибки в формате ProblemDetails с машинно-читаемым полем "code"
    public static class BankProblems
    {
        public static IResult Create(int status, string code, string title) =>
            Results.Problem(
                title: title,
                statusCode: status,
                extensions: new Dictionary<string, object?> { ["code"] = code });

        public static IResult NotFound(string what) =>
            Create(StatusCodes.Status404NotFound, "not_found", $"{what} not found");
    }
}
