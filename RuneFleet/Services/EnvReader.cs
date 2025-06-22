using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using RuneFleet.Interop;

namespace RuneFleet.Services
{
    public static class EnvReader
    {
        public static Dictionary<string, string> ReadEnvironmentVariablesFromProcess(int pid)
        {
            var envVars = new Dictionary<string, string>();

            using var hProcess = NativeMethods.OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead, false, pid);
            if (hProcess.IsInvalid)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to open process.");

            bool isWow64;
            if (!NativeMethods.IsWow64Process(hProcess.DangerousGetHandle(), out isWow64))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to check process bitness.");

            if (Environment.Is64BitOperatingSystem && !isWow64)
            {
                // 64-bit target process
                return ReadEnvironment64(hProcess);
            }
            else
            {
                // 32-bit target process
                return ReadEnvironment32(hProcess);
            }
        }

        private static Dictionary<string, string> ReadEnvironment64(SafeProcessHandle hProcess)
        {
            var envVars = new Dictionary<string, string>();

            PROCESS_BASIC_INFORMATION64 pbi = new PROCESS_BASIC_INFORMATION64();
            int returnLength;
            int status = NativeMethods.NtQueryInformationProcess(hProcess.DangerousGetHandle(), 0, ref pbi, Marshal.SizeOf<PROCESS_BASIC_INFORMATION64>(), out returnLength);
            if (status != 0)
                throw new Win32Exception(status, "Failed to query process information.");

            IntPtr rtlUserProcParamsAddress = ReadPointer64(hProcess, new IntPtr((long)pbi.PebBaseAddress + 0x20));
            IntPtr environmentAddress = ReadPointer64(hProcess, rtlUserProcParamsAddress + 0x80);

            byte[] buffer = new byte[0x10000];
            if (!NativeMethods.ReadProcessMemory(hProcess, environmentAddress, buffer, buffer.Length, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to read environment block.");

            string envBlock = System.Text.Encoding.Unicode.GetString(buffer);
            var entries = envBlock.Split('\0', StringSplitOptions.RemoveEmptyEntries);
            foreach (var entry in entries)
            {
                int idx = entry.IndexOf('=');
                if (idx > 0)
                {
                    string key = entry.Substring(0, idx);
                    string val = entry.Substring(idx + 1);
                    envVars[key] = val;
                }
            }

            return envVars;
        }

        private static Dictionary<string, string> ReadEnvironment32(SafeProcessHandle hProcess)
        {
            var envVars = new Dictionary<string, string>();

            PROCESS_BASIC_INFORMATION32 pbi = new PROCESS_BASIC_INFORMATION32();
            int returnLength;
            int status = NativeMethods.NtQueryInformationProcess32(hProcess.DangerousGetHandle(), 0, ref pbi, Marshal.SizeOf<PROCESS_BASIC_INFORMATION32>(), out returnLength);
            if (status != 0)
                throw new Win32Exception(status, "Failed to query process information.");

            IntPtr rtlUserProcParamsAddress = ReadPointer32(hProcess, new IntPtr(pbi.PebBaseAddress.ToInt32() + 0x10));
            IntPtr environmentAddress = ReadPointer32(hProcess, new IntPtr(rtlUserProcParamsAddress.ToInt32() + 0x48));

            byte[] buffer = new byte[0x10000];
            if (!NativeMethods.ReadProcessMemory(hProcess, environmentAddress, buffer, buffer.Length, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to read environment block.");

            string envBlock = System.Text.Encoding.Unicode.GetString(buffer);
            var entries = envBlock.Split('\0', StringSplitOptions.RemoveEmptyEntries);
            foreach (var entry in entries)
            {
                int idx = entry.IndexOf('=');
                if (idx > 0)
                {
                    string key = entry.Substring(0, idx);
                    string val = entry.Substring(idx + 1);
                    envVars[key] = val;
                }
            }

            return envVars;
        }

        private static IntPtr ReadPointer64(SafeProcessHandle hProcess, IntPtr address)
        {
            byte[] buffer = new byte[8];
            if (!NativeMethods.ReadProcessMemory(hProcess, address, buffer, buffer.Length, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to read pointer.");

            return new IntPtr(BitConverter.ToInt64(buffer, 0));
        }

        private static IntPtr ReadPointer32(SafeProcessHandle hProcess, IntPtr address)
        {
            byte[] buffer = new byte[4];
            if (!NativeMethods.ReadProcessMemory(hProcess, address, buffer, buffer.Length, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to read pointer.");

            return new IntPtr(BitConverter.ToInt32(buffer, 0));
        }

    }

}
