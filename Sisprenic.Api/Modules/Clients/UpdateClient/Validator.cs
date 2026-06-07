using FluentValidation;

namespace Sisprenic.Api.Modules.Clients.UpdateClient;

public class UpdateClientValidator : AbstractValidator<UpdateClientRequest>
{
    public UpdateClientValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                x.FirstName is not null ||
                x.SecondName is not null ||
                x.LastName is not null ||
                x.SecondLastName is not null ||
                x.Identification is not null ||
                x.PhoneNumber is not null)
            .WithMessage("Debe enviar al menos una propiedad para actualizar.");

        When(x => x.FirstName is not null, () =>
        {
            RuleFor(x => x.FirstName!)
                .NotEmpty().WithMessage("El primer nombre no puede estar vacío.")
                .MaximumLength(100).WithMessage("El primer nombre no puede exceder los 100 caracteres.");
        });

        When(x => x.SecondName is not null, () =>
        {
            RuleFor(x => x.SecondName!)
                .MaximumLength(100).WithMessage("El segundo nombre no puede exceder los 100 caracteres.");
        });

        When(x => x.LastName is not null, () =>
        {
            RuleFor(x => x.LastName!)
                .NotEmpty().WithMessage("El apellido no puede estar vacío.")
                .MaximumLength(100).WithMessage("El apellido no puede exceder los 100 caracteres.");
        });

        When(x => x.SecondLastName is not null, () =>
        {
            RuleFor(x => x.SecondLastName!)
                .MaximumLength(100).WithMessage("El segundo apellido no puede exceder los 100 caracteres.");
        });

        When(x => x.Identification is not null, () =>
        {
            RuleFor(x => x.Identification!)
                .NotEmpty().WithMessage("La identificación no puede estar vacía.")
                .MaximumLength(14).WithMessage("La identificación no puede exceder los 14 caracteres.");
        });

        When(x => x.PhoneNumber is not null, () =>
        {
            RuleFor(x => x.PhoneNumber!)
                .NotEmpty().WithMessage("El número de teléfono no puede estar vacío.")
                .MaximumLength(8).WithMessage("El número de teléfono no puede exceder los 8 caracteres.");
        });
    }
}
