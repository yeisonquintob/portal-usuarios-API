using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserPortal.Business.Services.Interfaces;
using UserPortal.Shared.DTOs.Request;
using UserPortal.Shared.DTOs.Response;
using UserPortal.Shared.Exceptions;
using UserPortal.Shared.Constants;

namespace UserPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginUserDTO loginDto)
    {
        try
        {
            _logger.LogInformation("Intento de login para usuario: {Username}", loginDto.Username);
            var result = await _authService.LoginAsync(loginDto);
            
            return Ok(ApiResponse<AuthResponseDTO>.CreateSuccess(
                result, 
                SuccessMessages.LoginSuccess));
        }
        catch (UnauthorizedException)
        {
            _logger.LogWarning("Credenciales inválidas para usuario: {Username}", loginDto.Username);
            return Unauthorized(ApiResponse<object>.CreateError(
                ErrorMessages.InvalidCredentials));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login de usuario: {Username}", loginDto.Username);
            throw;
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerDto)
    {
        try
        {
            var result = await _authService.RegisterAsync(registerDto);
            return Ok(ApiResponse<AuthResponseDTO>.CreateSuccess(
                result, 
                SuccessMessages.UserCreated));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<object>.CreateError(
                ex.Message,
                ex.Errors));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en registro de usuario");
            throw;
        }
    }
}