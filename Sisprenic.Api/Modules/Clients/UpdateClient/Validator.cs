using FluentValidation;

namespace Sisprenic.Api.Modules.Clients.UpdateClient;

public class UpdateClientValidator : AbstractValidator<UpdateClientRequest>
{
    public UpdateClientValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                x.FirstName.HasValue ||
                x.SecondName.HasValue ||
                x.LastName.HasValue ||
                x.SecondLastName.HasValue ||
                x.Identification.HasValue ||
                x.PhoneNumber.HasValue)
            .WithMessage("Debe enviar al menos una propiedad para actualizar.");

        When(x => x.FirstName.HasValue, () =>
        {
            RuleFor(x => x.FirstName.Value)
                .NotEmpty().WithMessage("El primer nombre no puede estar vacío.")
                .MaximumLength(100).WithMessage("El primer nombre no puede exceder los 100 caracteres.");
        });

        When(x => x.SecondName is { HasValue: true, Value: not null }, () =>
        {
            RuleFor(x => x.SecondName.Value)
                .MaximumLength(100).WithMessage("El segundo nombre no puede exceder los 100 caracteres.");
        });

        When(x => x.LastName.HasValue, () =>
        {
            RuleFor(x => x.LastName.Value)
                .NotEmpty().WithMessage("El apellido no puede estar vacío.")
                .MaximumLength(100).WithMessage("El apellido no puede exceder los 100 caracteres.");
        });

        When(x => x.SecondLastName is { HasValue: true, Value: not null }, () =>
        {
            RuleFor(x => x.SecondLastName.Value)
                .MaximumLength(100).WithMessage("El segundo apellido no puede exceder los 100 caracteres.");
        });

        When(x => x.Identification.HasValue, () =>
        {
            RuleFor(x => x.Identification.Value)
                .NotEmpty().WithMessage("La identificación no puede estar vacía.")
                .MaximumLength(14).WithMessage("La identificación no puede exceder los 14 caracteres.");
        });

        When(x => x.PhoneNumber.HasValue, () =>
        {
            RuleFor(x => x.PhoneNumber.Value)
                .NotEmpty().WithMessage("El número de teléfono no puede estar vacío.")
                .MaximumLength(8).WithMessage("El número de teléfono no puede exceder los 8 caracteres.");
        });
    }
}
