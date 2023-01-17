namespace SafeShare.DTO;

public record UserSecretDto
{
    public required string Token { get; init; }
}

