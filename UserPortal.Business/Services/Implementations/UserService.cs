using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using UserPortal.Business.Services.Interfaces;
using UserPortal.Data.Entities;
using UserPortal.Data.Infrastructure;
using UserPortal.Data.UnitOfWork;
using UserPortal.Shared.Constants;
using UserPortal.Shared.DTOs.Request;
using UserPortal.Shared.DTOs.Response;
using UserPortal.Shared.Exceptions;
using UserPortal.Shared.Models;

namespace UserPortal.Business.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UserService> _logger;
    private const int MaxProfilePictureSize = 5 * 1024 * 1024; // 5MB
    private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png", "image/gif" };

    public UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserResponseDTO> GetUserByIdAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Obteniendo usuario por ID: {UserId}", userId);
            
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado: {UserId}", userId);
                throw new NotFoundException(nameof(User), userId);
            }

            var userDto = _mapper.Map<UserResponseDTO>(user);
            _logger.LogInformation("Usuario encontrado exitosamente: {UserId}", userId);
            
            return userDto;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error obteniendo usuario por ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<PaginatedResult<UserResponseDTO>> GetUsersAsync(PaginationParams parameters)
    {
        try
        {
            _logger.LogInformation("Obteniendo usuarios paginados. Página: {PageNumber}, Tamaño: {PageSize}",
                parameters.PageNumber, parameters.PageSize);

            parameters ??= new PaginationParams();

            var users = await _unitOfWork.Users.GetAllPaginatedAsync(parameters);
            
            if (!users.Items.Any())
            {
                _logger.LogInformation("No se encontraron usuarios");
                return PaginatedResult<UserResponseDTO>.Empty(parameters);
            }

            var userDtos = _mapper.Map<List<UserResponseDTO>>(users.Items);
            var result = new PaginatedResult<UserResponseDTO>(userDtos, users.TotalItems, parameters);

            _logger.LogInformation("Se encontraron {TotalItems} usuarios", result.TotalItems);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo usuarios paginados");
            throw;
        }
    }

    public async Task<UserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO updateDto)
    {
        try
        {
            _logger.LogInformation("Actualizando usuario: {UserId}", userId);

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado para actualizar: {UserId}", userId);
                throw new NotFoundException(nameof(User), userId);
            }

            // Verificar email único
            if (!string.IsNullOrEmpty(updateDto.Email) && 
                updateDto.Email != user.Email)
            {
                if (await _unitOfWork.Users.IsEmailInUseAsync(updateDto.Email))
                {
                    _logger.LogWarning("Intento de actualizar a un email ya existente: {Email}", updateDto.Email);
                    throw new ValidationException(ErrorMessages.EmailAlreadyExists);
                }
                user.Email = updateDto.Email;
            }

            // Actualizar campos básicos
            if (!string.IsNullOrEmpty(updateDto.FirstName))
                user.FirstName = updateDto.FirstName;
            if (!string.IsNullOrEmpty(updateDto.LastName))
                user.LastName = updateDto.LastName;

            // Actualizar contraseña si se proporciona
            if (!string.IsNullOrEmpty(updateDto.CurrentPassword) && 
                !string.IsNullOrEmpty(updateDto.NewPassword))
            {
                var isValidPassword = _passwordHasher.VerifyPassword(
                    updateDto.CurrentPassword,
                    user.PasswordHash);

                if (!isValidPassword)
                {
                    _logger.LogWarning("Contraseña actual incorrecta para usuario: {UserId}", userId);
                    throw new ValidationException(ErrorMessages.CurrentPasswordIncorrect);
                }

                user.PasswordHash = _passwordHasher.HashPassword(updateDto.NewPassword);
                _logger.LogInformation("Contraseña actualizada para usuario: {UserId}", userId);
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var updatedUserDto = _mapper.Map<UserResponseDTO>(user);
            _logger.LogInformation("Usuario actualizado exitosamente: {UserId}", userId);
            
            return updatedUserDto;
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Error actualizando usuario: {UserId}", userId);
            throw;
        }
    }

    public async Task DeleteUserAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Eliminando usuario: {UserId}", userId);

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado para eliminar: {UserId}", userId);
                throw new NotFoundException(nameof(User), userId);
            }

            // Verificar si es el último administrador
            if (user.Role?.Name == UserRoles.Admin)
            {
                var adminCount = await _unitOfWork.Users
                    .GetQueryable()
                    .CountAsync(u => u.Role!.Name == UserRoles.Admin && u.IsActive);

                if (adminCount <= 1)
                {
                    _logger.LogWarning("Intento de eliminar el último administrador: {UserId}", userId);
                    throw new ValidationException("No se puede eliminar el último administrador del sistema");
                }
            }

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Usuario eliminado exitosamente: {UserId}", userId);
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Error eliminando usuario: {UserId}", userId);
            throw;
        }
    }

    public async Task<PaginatedResult<UserResponseDTO>> SearchUsersAsync(
        string searchTerm,
        PaginationParams parameters)
    {
        try
        {
            _logger.LogInformation("Buscando usuarios con término: {SearchTerm}", searchTerm);

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new ValidationException("El término de búsqueda no puede estar vacío");
            }

            parameters ??= new PaginationParams();

            var query = _unitOfWork.Users.GetQueryable()
                .Where(u => 
                    u.Username.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm) ||
                    u.FirstName.Contains(searchTerm) ||
                    u.LastName.Contains(searchTerm));

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip(parameters.Skip)
                .Take(parameters.PageSize)
                .ToListAsync();

            var userDtos = _mapper.Map<List<UserResponseDTO>>(items);
            var result = new PaginatedResult<UserResponseDTO>(userDtos, totalItems, parameters);

            _logger.LogInformation("Se encontraron {Count} usuarios con el término: {SearchTerm}", 
                result.TotalItems, searchTerm);

            return result;
        }
        catch (Exception ex) when (ex is not ValidationException)
        {
            _logger.LogError(ex, "Error buscando usuarios con término: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<string> UpdateProfilePictureAsync(int userId, IFormFile file)
    {
        try
        {
            _logger.LogInformation("Actualizando foto de perfil para usuario: {UserId}", userId);

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado: {UserId}", userId);
                throw new NotFoundException(nameof(User), userId);
            }

            ValidateProfilePicture(file);

            // Eliminar foto anterior si existe
            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                var oldPath = Path.Combine("wwwroot", user.ProfilePicture.TrimStart('/'));
                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                    _logger.LogInformation("Foto de perfil anterior eliminada: {Path}", oldPath);
                }
            }

            // Guardar nueva foto
            var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var relativePath = Path.Combine("uploads", "profile-pictures", fileName);
            var absolutePath = Path.Combine("wwwroot", relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
            
            using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Actualizar URL en base de datos
            user.ProfilePicture = $"/{relativePath.Replace('\\', '/')}";
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Foto de perfil actualizada exitosamente para usuario: {UserId}", userId);

            return user.ProfilePicture;
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Error actualizando foto de perfil para usuario: {UserId}", userId);
            throw;
        }
    }

    private void ValidateProfilePicture(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ValidationException("No se ha proporcionado ningún archivo");
        }

        if (!AllowedImageTypes.Contains(file.ContentType.ToLower()))
        {
            throw new ValidationException("El archivo debe ser una imagen (JPEG, PNG o GIF)");
        }

        if (file.Length > MaxProfilePictureSize)
        {
            throw new ValidationException($"El archivo no debe exceder {MaxProfilePictureSize / 1024 / 1024}MB");
        }
    }
}
