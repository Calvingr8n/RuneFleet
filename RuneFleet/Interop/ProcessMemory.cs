using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RuneFleet.Interop
{
    public enum ProcessAccessFlags : uint
    {
        QueryInformation = 0x0400,
        VirtualMemoryRead = 0x0010
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_BASIC_INFORMATION64
    {
        public ulong ExitStatus;
        public ulong PebBaseAddress;
        public ulong AffinityMask;
        public ulong BasePriority;
        public ulong UniqueProcessId;
        public ulong InheritedFromUniqueProcessId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_BASIC_INFORMATION32
    {
        public IntPtr ExitStatus;
        public IntPtr PebBaseAddress;
        public IntPtr AffinityMask;
        public IntPtr BasePriority;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;
    }
}
