using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using UserPortal.Business.Services.Interfaces;
using UserPortal.Data.Entities;
using UserPortal.Data.Infrastructure;
using UserPortal.Data.UnitOfWork;
using UserPortal.Shared.Constants;
using UserPortal.Shared.DTOs.Request;
using UserPortal.Shared.DTOs.Response;
using UserPortal.Shared.Exceptions;
using UserPortal.Shared.Helpers;
using UserPortal.Shared.Models;

namespace UserPortal.Business.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthService> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuthService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        ILogger<AuthService> logger,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginUserDTO loginDto)
    {
        try
        {
            _logger.LogInformation("Intento de login para usuario: {Username}", loginDto.Username);
            
            var user = await _unitOfWork.Users.GetByUsernameOrEmailAsync(loginDto.Username);
            
            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado: {Username}", loginDto.Username);
                throw new UnauthorizedException(ErrorMessages.InvalidCredentials);
            }

            bool isValidPassword = _passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                _logger.LogWarning("Contraseña inválida para usuario: {Username}", loginDto.Username);
                throw new UnauthorizedException(ErrorMessages.InvalidCredentials);
            }

            // Actualizar último login
            await _unitOfWork.Users.UpdateLastLoginAsync(user.Id);

            // Preparar modelo para JWT
            var userModel = new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RoleName = user.Role?.Name ?? UserRoles.User
            };

            // Generar tokens
            var accessToken = JwtHelper.GenerateJwtToken(userModel);
            var refreshToken = await CreateRefreshTokenAsync(user.Id);

            var response = new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = refreshToken.ExpiresAt,
                User = _mapper.Map<UserResponseDTO>(user)
            };

            await _unitOfWork.SaveChangesAsync();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login para usuario: {Username}", loginDto.Username);
            throw;
        }
    }

    public async Task<AuthResponseDTO> RegisterAsync(RegisterUserDTO registerDto)
    {
        try
        {
            // Verificar si el usuario ya existe
            var existingUser = await _unitOfWork.Users.GetByUsernameOrEmailAsync(registerDto.Username);
            if (existingUser != null)
            {
                throw new ValidationException(ErrorMessages.UsernameAlreadyExists);
            }

            // Verificar si el email ya existe
            if (await _unitOfWork.Users.IsEmailInUseAsync(registerDto.Email))
            {
                throw new ValidationException(ErrorMessages.EmailAlreadyExists);
            }

            // Obtener rol por defecto
            var defaultRole = await _unitOfWork.Roles.GetDefaultRoleAsync()
                ?? throw new NotFoundException("Default role not found");

            // Crear hash de la contraseña
            string passwordHash = _passwordHasher.HashPassword(registerDto.Password);

            // Crear nuevo usuario
            var newUser = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                RoleId = defaultRole.Id,
                CreatedAt = _dateTimeProvider.UtcNow
            };

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            // Preparar modelo para JWT
            var userModel = new UserModel
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email,
                RoleName = defaultRole.Name
            };

            // Generar tokens
            var accessToken = JwtHelper.GenerateJwtToken(userModel);
            var refreshToken = await CreateRefreshTokenAsync(newUser.Id);

            var response = new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = refreshToken.ExpiresAt,
                User = _mapper.Map<UserResponseDTO>(newUser)
            };

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Usuario registrado exitosamente: {Username}", registerDto.Username);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registrando nuevo usuario: {Username}", registerDto.Username);
            throw;
        }
    }

    private async Task<RefreshToken> CreateRefreshTokenAsync(int userId)
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = _dateTimeProvider.UtcNow.AddDays(7),
            UserId = userId,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.Users.AddRefreshTokenAsync(refreshToken);
        return refreshToken;
    }
}