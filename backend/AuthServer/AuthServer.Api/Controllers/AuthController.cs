using AuthServer.Core.DTOs;
using AuthServer.Core.Interfaces.Services;
using AuthServer.Domain.Entity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Api.Controllers;

public class AuthController : Controller
{
    private readonly IJwtService _jwtService;

    public AuthController(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Invalid login request.");
        }

        var user = new User(request.Email, request.Password);

        // Generate tokens
        try
        {
            var (accessToken, refreshToken) = _jwtService.GenerateTokens(user);

            // Return tokens
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }
        catch (Exception ex)
        {
            return ex is UnauthorizedAccessException
                ? Unauthorized("Authentication failed.")
                : StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost]
    [Route("refresh")]
    public IActionResult Refresh([FromBody] RefreshTokenRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest("Invalid refresh token request.");
        }

        // Validate and refresh the token
        try
        {
            var refreshTokenResponse = new RefreshTokenResponse
                { AccessToken = _jwtService.FreshToken(request.RefreshToken) };
            return Ok(refreshTokenResponse);
        }
        catch (Exception ex)
        {
            return ex is UnauthorizedAccessException
                ? Unauthorized("Invalid refresh token.")
                : StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost]
    [Route("validate")]
    public IActionResult Validate([FromBody] ValidateToken request)
    {
        if (request == null || string.IsNullOrEmpty(request.Token))
        {
            return BadRequest("Invalid token request.");
        }

        // Validate the token
        try
        {
            var isValid = _jwtService.ValidateToken(request.Token);
            var response = new ValidateResponse
            {
                IsValid = isValid
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ex is UnauthorizedAccessException
                ? Unauthorized("Invalid token.")
                : StatusCode(500, "An unexpected error occurred.");
        }
    }
}