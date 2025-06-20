using OSRSClientManager.Models;

namespace OSRSClientManager.Services
{
    internal class AccountLoader
    {
        public static List<Account> LoadFromCsv(string path)
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
                    Group = parts[5],
                    Client = parts[6],
                    Arguments = parts[7]
                });
            }
            return accounts;
        }
    }
}
