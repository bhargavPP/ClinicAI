using ClinicAI.Application.DTOs;
using ClinicAI.Application.Features.Users.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        SetCookies(data);

        return Ok(new { user = data.User });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return Unauthorized(result.Message);

        var data = result.Data;

        SetCookies(data);

        return Ok(new { user = data.User });
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

        await _mediator.Send(new LogoutCommand(refreshToken));

        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");

        return Ok();
    }

    private void SetCookies(AuthResponse data)
    {
        Response.Cookies.Append("accessToken", data.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(1),
            Domain = "clinic-ai-api.azurewebsites.net"
        });

        Response.Cookies.Append("refreshToken", data.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }
}