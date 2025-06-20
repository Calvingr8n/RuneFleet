using OSRSClientManager.Interop;
using OSRSClientManager.Models;
using System.Diagnostics;

namespace OSRSClientManager.Services
{
    internal class ClientHelper
    {

        public static async void LaunchClient(Account acc)
        {
            var filename = acc.Client?.ToString();
            if (string.IsNullOrEmpty(filename))
            {
                filename = @"C:\Program Files (x86)\Jagex Launcher\Games\Old School RuneScape\Client\osclient.exe";
            }
            var psi = new ProcessStartInfo
            {
                // Made to swap with any client
                FileName = filename,
                UseShellExecute = false
            };
            psi.EnvironmentVariables["JX_ACCESS_TOKEN"] = acc.AccessToken;
            psi.EnvironmentVariables["JX_REFRESH_TOKEN"] = acc.RefreshToken;
            psi.EnvironmentVariables["JX_SESSION_ID"] = acc.SessionId;
            psi.EnvironmentVariables["JX_CHARACTER_ID"] = acc.CharacterId;
            psi.EnvironmentVariables["JX_DISPLAY_NAME"] = acc.DisplayName;
            psi.Arguments = acc.Arguments?.ToString() ?? "";

            var proc = Process.Start(psi);

            if (filename.Contains("osclient", StringComparison.OrdinalIgnoreCase))
            {
                acc.Pid = proc?.Id;
            }
            else
            {
                // runelite
                var childPid = await RuneLiteProcessHelper.WaitForChildRuneLiteAsync(proc.Id);
                acc.Pid = childPid;
            }

        }

        public static void FocusWindowByPid(int pid)
        {
            try
            {
                var proc = Process.GetProcessById(pid);
                NativeMethods.ShowWindow(proc.MainWindowHandle, NativeMethods.SW_RESTORE);
                NativeMethods.SetForegroundWindow(proc.MainWindowHandle);
            }
            catch (Exception ex)
            {
                Console.WriteLine("FocusWindowByPid: " + ex.ToString());
            }
        }

        public static void ReplaceWorldIds(string defaltWorld, string lastWorld)
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filePath = Path.Combine(userProfile, @"Jagex\Old School Runescape\preferences_client.dat");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            var lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("DefaultWorldId"))
                {

                    lines[i] = $"DefaultWorldId {defaltWorld}";

                }
                else if (lines[i].StartsWith("LastWorldId"))
                {
                    lines[i] = $"LastWorldId {lastWorld}";
                }
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}
