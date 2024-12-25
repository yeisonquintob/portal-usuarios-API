// Ruta: ./UserPortal.Business/Validators/RegisterUserValidator.cs
using FluentValidation;
using UserPortal.Shared.DTOs.Request;
using UserPortal.Shared.Constants;

namespace UserPortal.Business.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserDTO>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .Length(3, DatabaseConstants.FieldLengths.Username)
                .WithMessage(ErrorMessages.InvalidLengthError("Username", 3, DatabaseConstants.FieldLengths.Username))
            .Matches("^[a-zA-Z0-9._-]+$")
                .WithMessage("El nombre de usuario solo puede contener letras, números y los caracteres . _ -");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .EmailAddress().WithMessage(ErrorMessages.InvalidEmail)
            .MaximumLength(DatabaseConstants.FieldLengths.Email)
                .WithMessage($"El email no puede exceder {DatabaseConstants.FieldLengths.Email} caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número")
            .MaximumLength(DatabaseConstants.FieldLengths.Password)
                .WithMessage($"La contraseña no puede exceder {DatabaseConstants.FieldLengths.Password} caracteres");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .Equal(x => x.Password).WithMessage(ErrorMessages.PasswordMismatch);

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(DatabaseConstants.FieldLengths.Name)
                .WithMessage($"El nombre no puede exceder {DatabaseConstants.FieldLengths.Name} caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(DatabaseConstants.FieldLengths.Name)
                .WithMessage($"El apellido no puede exceder {DatabaseConstants.FieldLengths.Name} caracteres");
    }
}