using FluentValidation;

using sisprenic_backend.Dtos.Loans;

namespace sisprenic_backend.Validators;

public class CreateLoanValidator : AbstractValidator<CreateLoanDto>
{
    public CreateLoanValidator()
    {
        RuleFor(x => x.Principal)
            .NotNull().WithMessage("El capital es requerido.")
            .GreaterThan(0).WithMessage("El capital debe ser mayor a cero.");

        RuleFor(x => x.InterestRate)
            .NotNull().WithMessage("La tasa de interés es requerida.")
            .GreaterThanOrEqualTo(0).WithMessage("La tasa de interés no puede ser negativa.")
            .LessThanOrEqualTo(100).WithMessage("La tasa de interés no puede superar el 100%.");

        RuleFor(x => x.TermMonths)
            .NotNull().WithMessage("El plazo es requerida.")
            .GreaterThan(0).WithMessage("El plazo debe ser al menos un mes.");
        
        RuleFor(x => x.StartDate)
            .NotNull().WithMessage("La fecha es requerida.");

        RuleFor(x => x.ClientId)
            .NotNull().WithMessage("El id del cliente es requerido.")
            .GreaterThan(0).WithMessage("El id del cliente debe ser un número positivo.");
    }
}
