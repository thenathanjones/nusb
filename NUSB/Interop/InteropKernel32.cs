using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace NUSB.Interop
{
    /// <summary>
    /// Contains the Windows interop functions related to kernel32.dll
    /// </summary>
    internal sealed class InteropKernel32
    {
        internal const uint OVERLAPPED = 0x40000000;

        [Flags]
        internal enum AccessRights : uint
        {
            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_ALL = 0x10000000
        }

        /// <summary>
        /// Share Modes.  See MSDN for more information.
        /// http://msdn.microsoft.com/en-us/library/aa363858.aspx
        /// </summary>
        [Flags]
        internal enum ShareModes : uint
        {
            FILE_SHARE_READ = 0x00000001,
            FILE_SHARE_WRITE = 0x00000002,
            FILE_SHARE_DELETE = 0x00000004
        }


        /// <summary>
        /// File Attribute Flags.  See MSDN for more information.
        /// http://msdn.microsoft.com/en-us/library/aa363858.aspx
        /// </summary>
        [Flags]
        internal enum FileAttributeFlags : uint
        {
            FILE_ATTRIBUTE_ARCHIVE = 0x20,
            FILE_ATTRIBUTE_ENCRYPTED = 0x4000,
            FILE_ATTRIBUTE_HIDDEN = 0x2,
            FILE_ATTRIBUTE_NORMAL = 0x80,
            FILE_ATTRIBUTE_OFFLINE = 0x1000,
            FILE_ATTRIBUTE_READONLY = 0x1,
            FILE_ATTRIBUTE_SYSTEM = 0x4,
            FILE_ATTRIBUTE_TEMPORARY = 0x100,
            FILE_FLAG_OVERLAPPED = 0x40000000,
            FILE_FLAG_NO_BUFFERING = 0x20000000
        }

        internal enum CreationDispositions : int
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXISTING = 5
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern SafeFileHandle CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool CloseHandle(
            SafeHandle handle
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool CancelIo(
            SafeHandle handle
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        unsafe internal static extern bool DeviceIoControl(
            SafeHandle hDevice,
            uint dwIoControlCode,
            byte* lpInBuffer,
            uint nInBufferSize,
            byte* lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            NativeOverlapped* lpOverlapped
            );


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        unsafe internal static extern bool WriteFile(
            SafeHandle hDevice,
            byte* lpBuffer,
            uint nNumberOfBytesToWrite,
            ref uint lpNumberOfBytesWritten,
            NativeOverlapped* lpOverlapped
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        unsafe internal static extern bool ReadFile(
            SafeHandle hDevice,
            byte* lpBuffer,
            uint nNumberOfBytesToRead,
            ref uint lpNumberOfBytesRead,
            NativeOverlapped* lpOverlapped
            );

    }
}
