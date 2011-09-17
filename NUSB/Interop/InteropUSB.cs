using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NUSB.Interop
{
    internal sealed class InteropUSB
    {
        /// <summary>
        /// Known Win32 Messages that we will handle
        /// </summary>
        [Flags]
        public enum Win32Error
        {
            ERROR_IO_PENDING = 997
        }

        /// <summary>
        /// SetupDiGetClassDevs Flags from SetupAPI.h
        /// </summary>
        [Flags]
        public enum DIGCFFlags
        {
            DIGCF_DEFAULT = 0x00000001,
            DIGCF_PRESENT = 0x00000002,
            DIGCF_ALLCLASSES = 0x00000004,
            DIGCF_PROFILE = 0x00000008,
            DIGCF_DEVICEINTERFACE = 0x00000010
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVICE_ATTRIBUTES
        {
            public Int32 Size;
            public Int16 VendorID;
            public Int16 ProductID;
            public Int16 VersionNumber;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public Int32 cbSize;
            public Guid InterfaceClassGuid;
            public Int32 Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public Int32 cbSize;
            public String DevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public Int32 cbSize;
            public Guid ClassGuid;
            public Int32 DevInst;
            public Int32 Reserved;
        }
    }
}
