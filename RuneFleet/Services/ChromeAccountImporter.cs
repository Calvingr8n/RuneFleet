using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WebSocketSharp;
using RuneFleet.Models;

namespace RuneFleet.Services
{
    internal class ChromeAccountImporter
    {
        private const string ChromeArgs = "--new-window https://account.jagex.com/en-GB/manage --incognito --remote-debugging-port=9222";
        private readonly HttpClient httpClient = new();

        private class AccountDto
        {
            public int JX_CHARACTER_ID { get; set; }
            public string? JX_DISPLAY_NAME { get; set; }
        }

        public async Task<List<Account>> ImportAsync()
        {
            _ = Process.Start("chrome.exe", ChromeArgs);

            string? wsUrl = null;
            for (int i = 0; i < 60 && wsUrl == null; i++)
            {
                await Task.Delay(1000);
                try
                {
                    var json = await httpClient.GetStringAsync("http://localhost:9222/json");
                    using var doc = JsonDocument.Parse(json);
                    foreach (var page in doc.RootElement.EnumerateArray())
                    {
                        var url = page.GetProperty("url").GetString();
                        if (url != null && url.StartsWith("https://account.runescape.com/en-GB/game"))
                        {
                            wsUrl = page.GetProperty("webSocketDebuggerUrl").GetString();
                            break;
                        }
                    }
                }
                catch
                {
                    // ignore until Chrome is ready
                }
            }

            if (wsUrl == null)
                throw new InvalidOperationException("Timed out waiting for page to load.");

            using var ws = new WebSocket(wsUrl);
            var tcs = new TaskCompletionSource<string>();
            ws.OnMessage += (s, e) =>
            {
                using var msgDoc = JsonDocument.Parse(e.Data);
                if (msgDoc.RootElement.TryGetProperty("id", out var idEl) && idEl.GetInt32() == 1)
                {
                    tcs.TrySetResult(e.Data);
                }
            };
            ws.Connect();

            const string script = "(function(){const accountDivs = document.querySelectorAll(\"div[data-testid^='game-accounts-section-game-account-item-'][data-rbd-draggable-id]\");const results = Array.from(accountDivs).map(div => { const id = div.getAttribute('data-rbd-draggable-id'); const nameDiv = div.querySelector(\"div[data-testid^='game-accounts-section-game-account-item-name-']\"); const name = nameDiv ? nameDiv.textContent.trim() : null; return { JX_CHARACTER_ID: parseInt(id,10), JX_DISPLAY_NAME: name };}).filter(entry => entry.JX_CHARACTER_ID && entry.JX_DISPLAY_NAME); return JSON.stringify(results);})();";
            var payload = JsonSerializer.Serialize(new
            {
                id = 1,
                method = "Runtime.evaluate",
                @params = new { expression = script, returnByValue = true }
            });
            ws.Send(payload);
            var response = await tcs.Task;
            ws.Close();

            using var respDoc = JsonDocument.Parse(response);
            var value = respDoc.RootElement.GetProperty("result").GetProperty("result").GetProperty("value").GetString();
            var accounts = value != null ? JsonSerializer.Deserialize<List<AccountDto>>(value) : new();
            return accounts?.Select(a => new Account { CharacterId = a.JX_CHARACTER_ID.ToString(), DisplayName = a.JX_DISPLAY_NAME }).ToList() ?? new();
        }
    }
}
