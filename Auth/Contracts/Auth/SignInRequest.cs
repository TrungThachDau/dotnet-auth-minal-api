namespace dotnet_auth.Contracts.Auth;

public record SignInRequest(
    string? Username,
    string? Email,
    string Password
);