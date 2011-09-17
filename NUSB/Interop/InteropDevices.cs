using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NUSB.Interop
{
    /// <summary>
    /// Contains the Windows interop functions related to devices
    /// </summary>
    internal sealed class InteropDevices
    {
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
