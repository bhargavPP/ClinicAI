namespace ClinicAI.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;
        public string? RevokedReason { get; set; }

        // FK
        public Guid UserId { get; set; }          // matches User.id type (Guid)
        public User User { get; set; } = null!;

        // Computed helpers (no DB column)
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
