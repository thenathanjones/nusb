using System;
using System.Collections.Generic;

namespace NUSB.Manager
{
    public interface IDeviceManager
    {
        /// <summary>
        /// Returns all of the USB devices that match the given criteria
        /// </summary>
        /// <param name="guid">Guid of the USB device</param>
        /// <param name="vendorId">Vendor ID of the USB device i.e. 0x0fc5</param>
        /// <param name="productId">Product ID of the USB device i.e. 0x0fc5</param>
        /// <returns>Path to the devices matching the criteria</returns>
        IEnumerable<string> FindDevices(Guid guid, string vendorId, string productId);

        /// <summary>
        /// Returns all of the USB devices that match the given criteria.
        /// </summary>
        /// <param name="guid">Guid of the USB device</param>
        /// <param name="vendorId">Vendor ID of the USB device i.e. 0x0fc5</param>
        /// <param name="productId">Product ID of the USB device i.e. 0x0fc5</param>
        /// <param name="deviceSerial">Optional. Unique identifier for a specific device.  If not specified, returns first match</param>
        /// <returns>Path to the devices matching the criteria</returns>
        string FindDevice(Guid guid, string vendorId, string productId, string deviceSerial);
    }
}
