using RuneFleet.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

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
                using var reader = new StreamReader(path);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound = null,
                    HeaderValidated = null,
                    BadDataFound = null
                };
                using var csv = new CsvReader(reader, config);
                csv.Context.RegisterClassMap<AccountMap>();
                return csv.GetRecords<Account>().ToList();
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

        private sealed class AccountMap : ClassMap<Account>
        {
            public AccountMap()
            {
                Map(m => m.AccessToken).Name("JX_ACCESS_TOKEN");
                Map(m => m.RefreshToken).Name("JX_REFRESH_TOKEN");
                Map(m => m.SessionId).Name("JX_SESSION_ID");
                Map(m => m.DisplayName).Name("JX_DISPLAY_NAME");
                Map(m => m.CharacterId).Name("JX_CHARACTER_ID");
                Map(m => m.Client).Name("Client");
                Map(m => m.Arguments).Name("Arguments");
                Map(m => m.Group).Convert(args =>
                    args.Row.GetField<string>("Group")?.Split(';', StringSplitOptions.RemoveEmptyEntries));
            }
        }
    }
}

