using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUSB.Interop;
using NUSB.Manager;

namespace USBFinderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IDeviceManager deviceManager = new DeviceManager();

            var deviceGuid = DeviceGuid.HID;

            var devices = deviceManager.FindDevices(deviceGuid, "0FC5", "B080");

            Console.WriteLine("Found " + devices.Count() + " devices");

            foreach (var devicePath in devices)
            {
                Console.WriteLine(devicePath);
            }

            Console.ReadKey();
        }
    }
}
