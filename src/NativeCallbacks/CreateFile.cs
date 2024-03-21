using System.Runtime.InteropServices;

namespace NativeCallbacks
{
    public static class CreateFile
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        delegate IntPtr CreateFileSig(
            IntPtr filename,
            uint access,
            uint share,
            IntPtr securityAttributes,
            uint creationDisposition,
            uint flagsAndAttributes,
            IntPtr templateFile);

        [UnmanagedCallersOnly(EntryPoint = "CreateFileW")]
        public static IntPtr MyCreateFile(
            IntPtr lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile)
        {
            Console.Out.WriteLine("Calling CreateFileW");

            // enumerate modules
            // find the kernel32 we want
            // get module offset
            // add address offset

            var library = LoadLibrary("kernel32.dll");
            var address = GetProcAddress(library, "CreateFileW");
            Console.Out.WriteLine($"Address: {address:X}");
            var module = GetModuleHandle("kernel32.dll");
            Console.Out.WriteLine($"Module: {module:X}");
            long functionAddress = long.Parse(Console.ReadLine(), System.Globalization.NumberStyles.HexNumber) + 0x23690;
            var function = Marshal.GetDelegateForFunctionPointer<CreateFileSig>(new IntPtr(functionAddress));

            IntPtr handle = function(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
            Console.Out.WriteLine($"Handle: {handle}");
            return handle;
        }
    }
}
