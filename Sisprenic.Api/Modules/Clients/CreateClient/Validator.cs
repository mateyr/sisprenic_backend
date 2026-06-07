using FluentValidation;

namespace Sisprenic.Api.Modules.Clients.CreateClient;

public class CreateClientValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El primer nombre es requerido.")
            .MaximumLength(100).WithMessage("El primer nombre no puede exceder los 100 caracteres.");

        RuleFor(x => x.SecondName)
            .MaximumLength(100).WithMessage("El segundo nombre no puede exceder los 100 caracteres.")
            .When(x => x.SecondName is not null);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(100).WithMessage("El apellido no puede exceder los 100 caracteres.");

        RuleFor(x => x.SecondLastName)
            .MaximumLength(100).WithMessage("El segundo apellido no puede exceder los 100 caracteres.")
            .When(x => x.SecondLastName is not null);

        RuleFor(x => x.Identification)
            .NotEmpty().WithMessage("La identificación es requerida.")
            .MaximumLength(50).WithMessage("La identificación no puede exceder los 50 caracteres.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("El número de teléfono es requerido.")
            .MaximumLength(30).WithMessage("El número de teléfono no puede exceder los 30 caracteres.");
    }
}
