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

    public UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<UserResponseDTO> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found", userId);
            }

            return _mapper.Map<UserResponseDTO>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<PaginatedResult<UserResponseDTO>> GetUsersAsync(PaginationParams parameters)
    {
        try
        {
            var users = await _unitOfWork.Users.GetAllPaginatedAsync(parameters);
            return _mapper.Map<PaginatedResult<UserResponseDTO>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated users");
            throw;
        }
    }

    public async Task<UserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO updateDto)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found", userId);
            }

            // Verificar email único
            if (!string.IsNullOrEmpty(updateDto.Email) && 
                updateDto.Email != user.Email && 
                await _unitOfWork.Users.IsEmailInUseAsync(updateDto.Email))
            {
                throw new ValidationException(ErrorMessages.EmailAlreadyExists);
            }

            // Actualizar campos básicos
            if (!string.IsNullOrEmpty(updateDto.Email))
                user.Email = updateDto.Email;
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
                    throw new ValidationException(ErrorMessages.CurrentPasswordIncorrect);
                }

                user.PasswordHash = _passwordHasher.HashPassword(updateDto.NewPassword);
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserResponseDTO>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", userId);
            throw;
        }
    }

    public async Task DeleteUserAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found", userId);
            }

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", userId);
            throw;
        }
    }

    public async Task<PaginatedResult<UserResponseDTO>> SearchUsersAsync(
        string searchTerm,
        PaginationParams parameters)
    {
        try
        {
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
            return new PaginatedResult<UserResponseDTO>(userDtos, totalItems, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<string> UpdateProfilePictureAsync(int userId, IFormFile file)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found", userId);
            }

            // Validar archivo
            if (file == null || file.Length == 0)
            {
                throw new ValidationException("No file was uploaded");
            }

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new ValidationException("File must be an image");
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB
            {
                throw new ValidationException("File size exceeds maximum limit (5MB)");
            }

            // Eliminar foto anterior si existe
            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                var oldPath = Path.Combine("wwwroot", user.ProfilePicture.TrimStart('/'));
                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
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

            return user.ProfilePicture;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile picture for user: {UserId}", userId);
            throw;
        }
    }
}