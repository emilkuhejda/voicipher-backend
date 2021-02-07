namespace Voicipher.Domain.Payloads
{
    public record GenerateTokenPayload
    {
        public string UserId { get; init; }

        public string Username { get; init; }

        public string Password { get; init; }

        public byte[] PasswordHash { get; init; }

        public byte[] PasswordSalt { get; init; }
    }
}
