using FluentValidation;

namespace Sisprenic.Api.Modules.Loans.UpdateLoan;

public class UpdateLoanValidator : AbstractValidator<UpdateLoanRequest>
{
    public UpdateLoanValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                x.Principal.HasValue ||
                x.InterestRate.HasValue ||
                x.TermMonths.HasValue ||
                x.StartDate.HasValue ||
                x.ClientId.HasValue)
            .WithMessage("Debe enviar al menos una propiedad para actualizar.");

        When(x => x.Principal.HasValue, () =>
        {
            RuleFor(x => x.Principal.Value)
                .GreaterThan(0).WithMessage("El capital debe ser mayor a cero.");
        });

        When(x => x.InterestRate.HasValue, () =>
        {
            RuleFor(x => x.InterestRate.Value)
                .GreaterThanOrEqualTo(0).WithMessage("La tasa de interés no puede ser negativa.")
                .LessThanOrEqualTo(100).WithMessage("La tasa de interés no puede superar el 100%.");
        });

        When(x => x.TermMonths.HasValue, () =>
        {
            RuleFor(x => x.TermMonths.Value)
                .GreaterThan(0).WithMessage("El plazo debe ser al menos un mes.");
        });

        When(x => x.ClientId.HasValue, () =>
        {
            RuleFor(x => x.ClientId.Value)
                .GreaterThan(0).WithMessage("El id del cliente debe ser un número positivo.");
        });
    }
}
