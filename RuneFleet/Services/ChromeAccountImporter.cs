using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using WebSocketSharp;
using RuneFleet.Models;

namespace RuneFleet.Services
{
    internal class ChromeAccountImporter
    {
        private const string ChromePath = @"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
        private readonly HttpClient httpClient = new();
        private readonly string userDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".runefleet",
            "chrome");
        private Process? chromeProcess;

        private class AccountDto
        {
            public int JX_CHARACTER_ID { get; set; }
            public string? JX_DISPLAY_NAME { get; set; }
        }

        public void LaunchBrowser()
        {
            if (chromeProcess == null || chromeProcess.HasExited)
            {
                Directory.CreateDirectory(userDataDir);
                var args = "--new-window https://account.jagex.com/en-GB/manage --incognito --remote-debugging-port=9222 " +
                           $"--user-data-dir=\"{userDataDir}\"";
                chromeProcess = Process.Start(ChromePath, args);
            }
        }

        public async Task<List<Account>> ImportAsync()
        {
            string? wsUrl = null;
            for (int attempt = 0; attempt < 30 && wsUrl == null; attempt++)
            {
                var json = await httpClient.GetStringAsync("http://localhost:9222/json");
                using var doc = JsonDocument.Parse(json);
                foreach (var page in doc.RootElement.EnumerateArray())
                {
                    var url = page.GetProperty("url").GetString();
                    if (!string.IsNullOrEmpty(url) && url.StartsWith("https://account.runescape.com/en-GB/game"))
                    {
                        wsUrl = page.GetProperty("webSocketDebuggerUrl").GetString();
                        break;
                    }
                }

                if (wsUrl == null)
                    await Task.Delay(1000);
            }

            if (wsUrl == null)
                throw new InvalidOperationException("RuneScape account page not found.");

            using var ws = new WebSocket(wsUrl);
            ws.Connect();

            async Task<string> EvalAsync(string expression, int id)
            {
                var tcsEval = new TaskCompletionSource<string>();
                void handler(object? _, MessageEventArgs e)
                {
                    using var msgDoc = JsonDocument.Parse(e.Data);
                    if (msgDoc.RootElement.TryGetProperty("id", out var idEl) && idEl.GetInt32() == id)
                    {
                        ws.OnMessage -= handler;
                        tcsEval.TrySetResult(e.Data);
                    }
                }
                ws.OnMessage += handler;
                var payload = JsonSerializer.Serialize(new { id, method = "Runtime.evaluate", @params = new { expression, returnByValue = true } });
                ws.Send(payload);
                return await tcsEval.Task;
            }

            int msgId = 1;
            for (int i = 0; i < 60; i++)
            {
                var readyResp = await EvalAsync("document.readyState", msgId++);
                using var readyDoc = JsonDocument.Parse(readyResp);
                var state = readyDoc.RootElement.GetProperty("result").GetProperty("result").GetProperty("value").GetString();
                if (state == "complete")
                    break;
                await Task.Delay(500);
            }

            const string script = "(function(){const accountDivs = document.querySelectorAll(\"div[data-testid^='game-accounts-section-game-account-item-'][data-rbd-draggable-id]\");const results = Array.from(accountDivs).map(div => { const id = div.getAttribute('data-rbd-draggable-id'); const nameDiv = div.querySelector(\"div[data-testid^='game-accounts-section-game-account-item-name-']\"); const name = nameDiv ? nameDiv.textContent.trim() : null; return { JX_CHARACTER_ID: parseInt(id,10), JX_DISPLAY_NAME: name };}).filter(entry => entry.JX_CHARACTER_ID && entry.JX_DISPLAY_NAME); return JSON.stringify(results);})();";
            var response = await EvalAsync(script, msgId++);
            ws.Close();

            using var respDoc = JsonDocument.Parse(response);
            var value = respDoc.RootElement.GetProperty("result").GetProperty("result").GetProperty("value").GetString();
            var accounts = value != null ? JsonSerializer.Deserialize<List<AccountDto>>(value) : new();

            if (chromeProcess != null && !chromeProcess.HasExited)
            {
                try { chromeProcess.Kill(); } catch { }
                chromeProcess = null;
            }

            return accounts?.Select(a => new Account { CharacterId = a.JX_CHARACTER_ID.ToString(), DisplayName = a.JX_DISPLAY_NAME }).ToList() ?? new();
        }
    }
}
