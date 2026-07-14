using FluentResults;
using FluentValidation;

namespace UDBFRaceFlow.Application.Extensions.ValidatorExtensions
{
    public static class ValidatorExtensions
    {
        public static async Task<Result> ValidateDtoAsync<T>(this IValidator<T> validator, T instance, CancellationToken cancellationToken)
        {
            var validation = await validator.ValidateAsync(instance, cancellationToken);

            if (validation.IsValid)
            {
                return Result.Ok();
            }

            var errors = validation.Errors
                .Select(e => new Error(e.ErrorMessage)
                .WithMetadata("Property name", e.PropertyName))
                .ToList();

            return Result.Fail(errors);
        }
    }
}
