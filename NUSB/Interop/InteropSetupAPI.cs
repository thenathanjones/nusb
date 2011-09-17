using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NUSB.Interop
{
    internal sealed class InteropSetupAPI
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, Int32 Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern Int32 SetupDiCreateDeviceInfoList(ref Guid ClassGuid, Int32 hwndParent);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern Int32 SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref System.Guid InterfaceClassGuid, Int32 MemberIndex, ref USBDeclarations.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, Int32 Flags);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref InteropUSB.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, Int32 DeviceInterfaceDetailDataSize, ref Int32 RequiredSize, IntPtr DeviceInfoData);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean UnregisterDeviceNotification(IntPtr Handle);
    }
}
