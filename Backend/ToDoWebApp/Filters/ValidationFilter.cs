using FluentValidation;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Find the argument matching the DTO type we are validating
        var argument = context.Arguments.OfType<T>().FirstOrDefault();

        if (argument is not null)
        {
            var validationResult = await _validator.ValidateAsync(argument);

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }
        }

        return await next(context);
    }
}