using RuneFleet.Models;

namespace RuneFleet.Services
{
    // This class is responsible for loading account data from a CSV file.
    internal class AccountLoader
    {
        public static List<Account> LoadFromCsv(string path)
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
                File.WriteAllText(path, "JX_ACCESS_TOKEN,JX_REFRESH_TOKEN,JX_SESSION_ID,JX_DISPLAY_NAME,JX_CHARACTER_ID,Group,Client,Arguments\r\n");
                MessageBox.Show("Couldn't find accounts.csv. \r\n" +
                                "A template has been created for you in the same folder. \r\n" +
                                "Read the guide for more info.", 
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return new List<Account>();
            }
        }
    }
}
