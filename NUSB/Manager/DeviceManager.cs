using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NUSB.Interop;

namespace NUSB.Manager
{
    public sealed class DeviceManager : IDeviceManager
    {
        public IEnumerable<string> FindDevices(Guid guid, string vendorId, string productId)
        {
            var deviceInfoSet = IntPtr.Zero;
            var deviceDetailData = IntPtr.Zero;
            var devices = new List<string>();

            try
            {
                deviceInfoSet = InteropSetupAPI.SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero,
                                                                    (int)InteropSetupAPI.DIGCFFlags.DIGCF_DEVICEINTERFACE |
                                                                    (int)InteropSetupAPI.DIGCFFlags.DIGCF_PRESENT);

                var deviceInterfaceData = new InteropDevices.SP_DEVICE_INTERFACE_DATA();
                deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

                // find all the devices of the selected vendor/product ID
                for (var i = 0; InteropSetupAPI.SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref guid, i, ref deviceInterfaceData); i++)
                {
                    var devicePath = CheckDevice(deviceInterfaceData, deviceInfoSet, out deviceDetailData, vendorId, productId);
                    if (devicePath != null) { devices.Add(devicePath); }
                }

                return devices;
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero)
                    InteropSetupAPI.SetupDiDestroyDeviceInfoList(deviceInfoSet);

                if (deviceDetailData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(deviceDetailData);
                }
            }
        }

        public string FindDevice(Guid guid, string vendorId, string productId, string deviceSerial)
        {
            var devices = FindDevices(guid, vendorId, productId);

            // look for specific if requested or return the first found
            return deviceSerial != null ? devices.Single(devicePath => devicePath.Contains(deviceSerial.ToLower())) : devices.First();
        }

        #region Private

        private string CheckDevice(InteropDevices.SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInfoSet, out IntPtr deviceDetailData, string vendorId, string productId)
        {
            var requiredBufferSize = -1;

            // get the required size for the buffer
            InteropSetupAPI.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero,
                                                            0, ref requiredBufferSize, IntPtr.Zero);

            // allocate memory using the required size
            deviceDetailData = Marshal.AllocHGlobal(requiredBufferSize);

            // store cbSize in the first bytes of the array, with size based on 32 or 64 bit system
            Marshal.WriteInt32(deviceDetailData, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

            // now we've allocated the buffer, pass it in to retrieve the information
            if (InteropSetupAPI.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData,
                                                                deviceDetailData, requiredBufferSize, ref requiredBufferSize,
                                                                IntPtr.Zero))
            {
                // ignore cbSize
                var pDevicePath = new IntPtr(deviceDetailData.ToInt32() + 4);

                // retrieve the path name
                var devicePathName = Marshal.PtrToStringAuto(pDevicePath);

                // append underscore to we match the correct thing
                var vendorIdString = ("vid_" + vendorId).ToLower();
                var productIdString = ("pid_" + productId).ToLower();
                if (devicePathName != null &&
                    devicePathName.Contains(vendorIdString) &&
                    devicePathName.Contains(productIdString))
                {
                    return devicePathName;
                }
            }

            // not our guy, bring out the next
            return null;
        }

        #endregion
    }
}
