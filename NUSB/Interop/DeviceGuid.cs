using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUSB.Interop
{
    public abstract class DeviceGuid
    {
        /// <summary>
        /// Guid for Human Interface Devices
        /// </summary>
        public static Guid HID = new Guid("4d1e55b2-f16f-11cf-88cb-001111000030");
    }
}
