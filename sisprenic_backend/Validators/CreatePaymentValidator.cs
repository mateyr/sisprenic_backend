using FluentValidation;

using sisprenic_backend.Dtos.Payments;

namespace sisprenic_backend.Validators;

public class CreatePaymentValidator: AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.Principal)
            .GreaterThanOrEqualTo(0).WithMessage("El capital no puede ser negativo.");
        RuleFor(x => x.Interest)
            .GreaterThanOrEqualTo(0).WithMessage("El interés no puede ser negativo.");
        RuleFor(x => x.PaymentDay)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("La fecha de pago no puede ser posterior a la fecha actual.");
        RuleFor(x => x.LoanId)
            .NotNull().WithMessage("El id del préstamo es obligatorio.")
            .GreaterThan(0).WithMessage("El id del préstamo debe ser un número positivo.");
        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("La nota no puede exceder los 500 caracteres.");
        RuleFor(x => x)
            .Must(x => x.Principal != 0 || x.Interest != 0).WithMessage("El capital y el interés no pueden ser ambos cero.");
    }
}
