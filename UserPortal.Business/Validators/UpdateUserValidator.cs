// Ruta: ./UserPortal.Business/Validators/UpdateUserValidator.cs
using FluentValidation;
using UserPortal.Shared.DTOs.Request;
using UserPortal.Shared.Constants;

namespace UserPortal.Business.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserDTO>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage(ErrorMessages.InvalidEmail)
            .MaximumLength(DatabaseConstants.FieldLengths.Email)
                .WithMessage($"El email no puede exceder {DatabaseConstants.FieldLengths.Email} caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.FirstName)
            .MaximumLength(DatabaseConstants.FieldLengths.Name)
                .WithMessage($"El nombre no puede exceder {DatabaseConstants.FieldLengths.Name} caracteres")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(DatabaseConstants.FieldLengths.Name)
                .WithMessage($"El apellido no puede exceder {DatabaseConstants.FieldLengths.Name} caracteres")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        When(x => !string.IsNullOrEmpty(x.CurrentPassword) || !string.IsNullOrEmpty(x.NewPassword), () => {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("La contraseña actual es requerida para cambiar la contraseña");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La nueva contraseña es requerida")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
                .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
                .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
                .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número")
                .MaximumLength(DatabaseConstants.FieldLengths.Password)
                    .WithMessage($"La contraseña no puede exceder {DatabaseConstants.FieldLengths.Password} caracteres");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("La confirmación de la nueva contraseña es requerida")
                .Equal(x => x.NewPassword).WithMessage(ErrorMessages.PasswordMismatch);
        });

        RuleFor(x => x.ProfilePicture)
            .MaximumLength(DatabaseConstants.FieldLengths.ProfilePicture)
                .WithMessage($"La URL de la imagen no puede exceder {DatabaseConstants.FieldLengths.ProfilePicture} caracteres")
            .When(x => !string.IsNullOrEmpty(x.ProfilePicture));
    }
}