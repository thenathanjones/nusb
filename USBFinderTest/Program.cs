using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUSB.Controller;
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

            var light = deviceManager.FindDevice(deviceGuid, "0FC5", "B080", null);

            Console.WriteLine("Found " + light);

            IUSBController controller = new Windows32USBController();
            controller.Initialise(light, false);

            Thread.Sleep(2000);

            // usb_control_msg(0x21, 0x09, 0x0635, 0x000, "\x65\x0C#{colour}\xFF\x00\x00\x00\x00", 0)
            // usb_control_msg(requesttype, request, value, index, bytes, timeout)

            var controlBytes = new byte[8];
            controlBytes[0] = 0x65;
            controlBytes[1] = 0x0C;
            controlBytes[2] = 0x02; // This is the LED byte
            controlBytes[3] = 0xFF;

            controller.WriteControlOverlapped(0x03, controlBytes);

            Console.ReadKey();
        }
    }
}
