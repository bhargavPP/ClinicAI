using ClinicAI.Application.common.Models;
using FluentValidation;
using MediatR;

namespace ClinicAI.Application.common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );

                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Any())
                {
                    var message = failures
                        .Select(f => f.ErrorMessage)
                        .First();

                    // ✅ Handle only Result<T>
                    if (typeof(TResponse).IsGenericType &&
                        typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        var resultType = typeof(TResponse);
                        var failureMethod = resultType.GetMethod("Failure");

                        var result = failureMethod?.Invoke(null, new object[] { message });

                        return (TResponse)result!;
                    }

                    // fallback if something unexpected
                    throw new ValidationException(failures);
                }
            }

            return await next();
        }
    }
}
