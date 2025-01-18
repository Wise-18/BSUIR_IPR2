namespace AuthDemo.API.Models
{
    public record UserModel
    {
        public required string Id { get; set; }
        public required string Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public record UserRegistrationModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
