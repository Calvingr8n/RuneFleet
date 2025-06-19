using OSRSClientManager.Interop;
using OSRSClientManager.Models;
using System.Diagnostics;

namespace OSRSClientManager.Services
{
    internal class ClientHelpers
    {

        public static void LaunchClient(Account acc)
        {
            var psi = new ProcessStartInfo
            {
                //TODO: Make this swap with RuneLine client?
                FileName = @"C:\Program Files (x86)\Jagex Launcher\Games\Old School RuneScape\Client\osclient.exe",
                UseShellExecute = false
            };
            psi.EnvironmentVariables["JX_ACCESS_TOKEN"] = acc.AccessToken;
            psi.EnvironmentVariables["JX_REFRESH_TOKEN"] = acc.RefreshToken;
            psi.EnvironmentVariables["JX_SESSION_ID"] = acc.SessionId;
            psi.EnvironmentVariables["JX_CHARACTER_ID"] = acc.CharacterId;
            psi.EnvironmentVariables["JX_DISPLAY_NAME"] = acc.DisplayName;

            
            var proc = Process.Start(psi);
            acc.Pid = proc?.Id;
            
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
