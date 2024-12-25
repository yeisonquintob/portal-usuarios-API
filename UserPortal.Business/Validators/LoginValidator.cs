// Ruta: ./UserPortal.Business/Validators/LoginValidator.cs
using FluentValidation;
using UserPortal.Shared.DTOs.Request;
using UserPortal.Shared.Constants;

namespace UserPortal.Business.Validators;

public class LoginValidator : AbstractValidator<LoginUserDTO>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(DatabaseConstants.FieldLengths.Username)
                .WithMessage($"El usuario no puede exceder {DatabaseConstants.FieldLengths.Username} caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(DatabaseConstants.FieldLengths.Password)
                .WithMessage($"La contraseña no puede exceder {DatabaseConstants.FieldLengths.Password} caracteres");
    }
}
