using ClinicAI.Application.DTOs;
using ClinicAI.Application.Features.Users.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        var data = result.Data;
        var message = result.Message;
        SetCookies(data);

        return Ok(new { user = data.User, message= message });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return Unauthorized(result.Message);

        var data = result.Data;
        var message= result.Message;
        SetCookies(data);

        return Ok(new { user = data.User , message= message });
    }
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Unauthorized();

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var name = User.FindFirstValue(ClaimTypes.Name); // ← add this

        if (userId == null) return Unauthorized();

        return Ok(new { userId, email, role, name });  
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        var result = await _mediator.Send(new RefreshTokenCommand(refreshToken));

        if (!result.IsSuccess)
            return Unauthorized(result.Message);

        var data = result.Data;

        SetCookies(data);

        return Ok(new { user = data.User });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("No refresh token");
        await _mediator.Send(new LogoutCommand(refreshToken));

        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");

        return Ok();
    }

    private void SetCookies(AuthResponse data)
    {
        var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        Response.Cookies.Append("accessToken", data.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(10)
            //,            Domain = "clinic-ai-api.azurewebsites.net"
        });

        Response.Cookies.Append("refreshToken", data.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }
}