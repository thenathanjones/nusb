using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUSB.Controller
{
    public interface IUSBController : IDisposable
    {
        /// <summary>
        /// Configure the controller with the USB device at the specified path and attempt to connect
        /// </summary>
        /// <param name="pathToDevice">The path to the device in Windows</param>
        /// <param name="separateHandles">Whether to use separate handles for reading and writing to device</param>
        void Initialise(string pathToDevice, bool separateHandles);

        /// <summary>
        /// Connect to handle to the attached USB device
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect the handle to the attached USB device
        /// </summary>
        void Disconnect();

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
        /// <param name="writeBuffer">Buffer containing the data for the operation</param>
        /// <param name="readBuffer">Buffer to receive the returned data</param>
        void ReadControl(uint controlCode, byte[] writeBuffer, byte[] readBuffer);

        /// <summary>
        /// Asynchronously read an IO control message from the attached USB device
        /// </summary>
        /// <param name="controlCode">IOCTL code to use</param>
        /// <param name="writeBuffer">Buffer containing the data for the operation</param>
        /// <param name="readBuffer">Buffer to receive the returned data</param>
        void ReadControlOverlapped(uint controlCode, byte[] writeBuffer, byte[] readBuffer);

        /// <summary>
        /// Synchronously write data to the attached USB device
        /// </summary>
        /// <param name="writeBuffer">Buffer containing the data for the operation</param>
        void Write(byte[] writeBuffer);

        /// <summary>
        /// Asynchronously write data to the attached USB device
        /// </summary>
        /// <param name="writeBuffer">Buffer containing the data for the operation</param>
        void WriteOverlapped(byte[] writeBuffer);

        /// <summary>
        /// Synchronously read data from the attached USB device
        /// </summary>
        /// <param name="readBuffer">Buffer to receive the returned data</param>
        void Read(byte[] readBuffer);

        /// <summary>
        /// Asynchronously read data from the attached USB device
        /// </summary>
        /// <param name="readBuffer">Buffer to receive the returned data</param>
        void ReadOverlapped(byte[] readBuffer);

        /// <summary>
        /// Send a feature report to the attached USB device
        /// </summary>
        /// <param name="reportBuffer">Buffer containing the report</param>
        void HidSetFeature(byte[] reportBuffer);
    }
}
