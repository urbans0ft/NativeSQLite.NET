using System;
using System.Runtime.InteropServices;

namespace UrbanSoft.Windows.Native
{
    internal static class Win32
    {
        [DllImport("kernel32.dll",
            EntryPoint = "LoadLibraryA", CharSet = CharSet.Ansi, ThrowOnUnmappableChar = true,
            SetLastError = true)]
        public static extern IntPtr LoadLibrary(
            [MarshalAs(UnmanagedType.LPStr)]string lpFileName);
    }
}
