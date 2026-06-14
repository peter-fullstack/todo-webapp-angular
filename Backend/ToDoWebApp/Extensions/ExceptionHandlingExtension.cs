using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public static class ExceptionHandlingExtensions
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app, IHostEnvironment environment)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                // Resolve the Problem Details service from the current request context
                var problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();

                // 1. Handle FluentValidation Exceptions (400 Bad Request)
                if (exception is ValidationException validationException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    var validationErrors = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                    await problemDetailsService.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = new HttpValidationProblemDetails(validationErrors)
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Validation Failed",
                            Detail = "One or more inputs failed validation rules."
                        }
                    });
                    return;
                }

                // 2. Handle generic system crashes (500 Internal Server Error)
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await problemDetailsService.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "An unexpected error occurred.",
                        Detail = environment.IsDevelopment() ? exception?.Message : null
                    }
                });
            });
        });
    }
}