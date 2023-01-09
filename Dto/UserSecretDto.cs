namespace SafeShare.Dto;

public record UserSecretDto
{
    public required string Token { get; init; }
}

