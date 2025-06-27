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
            Accounts = LoadFromCsv(path);
        }

        private static List<Account> LoadFromCsv(string path)
        {
            try
            {
                var accounts = new List<Account>();
                var lines = File.ReadAllLines(path);

                foreach (var line in lines.Skip(1))
                {
                    var parts = line.Split(',');
                    if (parts.Length < 8) continue;
                    accounts.Add(new Account
                    {
                        AccessToken = parts[0],
                        RefreshToken = parts[1],
                        SessionId = parts[2],
                        DisplayName = parts[3],
                        CharacterId = parts[4],
                        Group = parts[5].Split(";"),
                        Client = parts[6],
                        Arguments = parts[7]
                    });
                }
                return accounts;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error loading accounts from CSV: {ex.Message}");
                File.WriteAllText(path,
                    "JX_ACCESS_TOKEN,JX_REFRESH_TOKEN,JX_SESSION_ID,JX_DISPLAY_NAME,JX_CHARACTER_ID,Group,Client,Arguments\r\n");
                MessageBox.Show("Couldn't find accounts.csv. \r\n" +
                                "A template has been created for you in the same folder. \r\n" +
                                "Read the guide for more info.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return new List<Account>();
            }
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

