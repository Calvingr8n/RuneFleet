using RuneFleet.Models;

namespace RuneFleet.Services
{
    /// <summary>
    /// Provides basic account management operations for the UI.
    /// </summary>
    internal class AccountManager
    {
        public List<Account> Accounts { get; private set; } = new();

        public void Load(string path)
        {
            Accounts = AccountLoader.LoadFromCsv(path);
        }

        public IEnumerable<string> GetGroups()
        {
            return Accounts
                .Where(a => a.Group != null)
                .SelectMany(a => a.Group!)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Distinct();
        }

        public IEnumerable<Account> GetAccounts(string? group)
        {
            group ??= "All";
            return group == "All"
                ? Accounts
                : Accounts.Where(a => a.Group != null && a.Group.Contains(group));
        }

        public Account? GetAccountByDisplayName(string displayName)
        {
            return Accounts.FirstOrDefault(a => a.DisplayName == displayName);
        }
    }
}

