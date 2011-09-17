using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUSB.Controller
{
    public interface IUSBController : IDisposable
    {
        /// <summary>
        /// Returns all of the USB devices that match the given criteria
        /// </summary>
        /// <param name="guid">Guid of the USB device</param>
        /// <param name="vendorId">Vendor ID of the USB device i.e. 0x0fc5</param>
        /// <param name="productId">Product ID of the USB device i.e. 0x0fc5</param>
        /// <param name="deviceSerial">Optional. Unique identifier for a specific device</param>
        /// <returns>Path to the devices matching the criteria</returns>
        IEnumerable<string> FindDevices(Guid guid, string vendorId, string productId, string deviceSerial);

        /// <summary>
        /// Synchronously write an IO control message to the attached USB device
        /// </summary>
        /// <param name="controlCode">IOCTL code to use</param>
        /// <param name="writeBuffer">Buffer containing the data for the operation</param>
        void WriteControl(uint controlCode, byte[] writeBuffer);

        /// <summary>
        /// Asynchronously write an IO control message to the attached USB device
        /// </summary>
        /// <param name="controlCode">IOCTL code to use</param>
        /// <param name="writeBuffer">Buffer containing the data for the operation</param>
        void WriteControlOverlapped(uint controlCode, byte[] writeBuffer);

        /// <summary>
        /// Synchronously write an clear message to the attached USB device
        /// </summary>
        /// <param name="controlCode">IOCTL code to use</param>
        void WriteClear(uint controlCode);

        /// <summary>
        /// Synchronously read an IO control message from the attached USB device
        /// </summary>
        /// <param name="controlCode">IOCTL code to use</param>
        /// <param name="inputBuffer">Buffer containing the data for the operation</param>
        /// <param name="outputBuffer">Buffer to receive the returned data</param>
        void ReadControl(uint controlCode, byte[] inputBuffer, byte[] outputBuffer);

        /// <summary>
        /// Asynchronously read an IO control message from the attached USB device
        /// </summary>
        /// <param name="controlCode">IOCTL code to use</param>
        /// <param name="inputBuffer">Buffer containing the data for the operation</param>
        /// <param name="outputBuffer">Buffer to receive the returned data</param>
        void ReadControlOverlapped(uint controlCode, byte[] inputBuffer, byte[] outputBuffer);
    }
}
