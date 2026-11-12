namespace AuthService.Contracts
{
    public record RegisterRequest(string Login, string Password, string? Email);
    public record LoginRequest(string Login, string Password);
    public record RefreshTokenRequest(string RefreshToken);
    public record ChangePasswordRequest(string OldPassword, string NewPassword);

    public record AuthResponse(
    Guid UserId,
    string Login,
    string AccessToken,
    string RefreshToken);
    public record ErrorResponse(string Code, string Message);

}
