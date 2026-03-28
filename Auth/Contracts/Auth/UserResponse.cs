namespace dotnet_auth.Contracts.Auth;

public record UserResponse(
    int Id,
    string Username,
    string Name,
    string Email,
    DateTime CreatedAt
);
