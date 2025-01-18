namespace AuthDemo.API.Models
{
    public class KeycloakUser
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool Enabled { get; set; }
        public Dictionary<string, string[]>? Attributes { get; set; }
        public List<Credential>? Credentials { get; set; }
        public List<string>? RealmRoles { get; set; }
        public Dictionary<string, string[]>? ClientRoles { get; set; }
        public long? CreatedTimestamp { get; set; }
    }

    public class Credential
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
        public bool Temporary { get; set; }
    }
}