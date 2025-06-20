namespace RuneFleet.Models
{
    internal class Account
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? SessionId { get; set; }
        public string? DisplayName { get; set; }
        public string? CharacterId { get; set; }
        public string? Group { get; set; }
        public string? Client { get; set; }
        public string? Arguments { get; set; }
        public int? Pid { get; set; }
    }
}
