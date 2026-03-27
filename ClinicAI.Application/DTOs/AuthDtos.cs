namespace ClinicAI.Application.DTOs
{
    public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    UserDto User
);
    public record UserDto(
Guid Id,
string FullName,
string Email,
string Role
);

    public record RefreshTokenRequest(string RefreshToken);
}
