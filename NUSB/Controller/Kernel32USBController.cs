using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.SafeHandles;
using NUSB.Interop;

namespace NUSB.Controller
{
    public sealed class Kernel32USBController : IUSBController
    {
        private string _devicePath;
        private bool _separateHandles;
        private SafeFileHandle _writeHandle;
        private SafeFileHandle _readHandle;
        private SafeFileHandle _handle;

        /// <summary>
        /// Lock to control access to the handles.  To prevent closing a handle at the same time as cancelling a pending IO
        /// </summary>
        private readonly object _cancelIOLock = new object();

        public void Dispose()
        {
            Disconnect();
        }

        public void Initialise(string pathToDevice, bool separateHandles)
        {
            _devicePath = pathToDevice;
            _separateHandles = separateHandles;

            Connect();
        }

        public void Connect()
        {
            OpenHandle();
        }

        private void OpenHandle()
        {
            if (_separateHandles)
            {
                // open separate handles for reading and writing
                OpenSeparateHandles();
            }
            else
            {
                // open a handle which will handle both reading and writing
                OpenSingleHandle();
            }
        }

        private void CleanupHandles()
        {
            lock (_cancelIOLock)
            {
                CleanupHandle(_handle);
                CleanupHandle(_readHandle);
                CleanupHandle(_writeHandle);

                _handle = _readHandle = _writeHandle = null;
            }
        }

        private void CleanupHandle(SafeFileHandle handle)
        {
            if (handle != null && !handle.IsClosed)
            {
                InteropKernel32.CloseHandle(handle);
                handle.Close();
            }
        }

        private void OpenSingleHandle()
        {
            _handle = InteropKernel32.CreateFile(_devicePath,
                                                 (uint)InteropKernel32.AccessRights.GENERIC_READ |
                                                 (uint)InteropKernel32.AccessRights.GENERIC_WRITE,
                                                 (uint)InteropKernel32.ShareModes.FILE_SHARE_WRITE |
                                                 (uint)InteropKernel32.ShareModes.FILE_SHARE_READ,
                                                 IntPtr.Zero,
                                                 (uint)InteropKernel32.CreationDispositions.OPEN_EXISTING,
                                                 (uint)InteropKernel32.FileAttributeFlags.FILE_FLAG_OVERLAPPED |
                                                 (uint)InteropKernel32.FileAttributeFlags.FILE_FLAG_NO_BUFFERING,
                                                 IntPtr.Zero);

            if (_handle == null || _handle.IsInvalid)
            {
                CleanupHandles();

                throw new Exception("Oh noes Cap'n, something is wrong with the handles!");
            }

            // use this handle for both reads and writes
            _readHandle = _handle;
            _writeHandle = _handle;
        }

        private void OpenSeparateHandles()
        {
            // open a handle for writing
            _writeHandle = InteropKernel32.CreateFile(_devicePath,
                                                      (uint)InteropKernel32.AccessRights.GENERIC_WRITE,
                                                      (uint)InteropKernel32.ShareModes.FILE_SHARE_WRITE |
                                                      (uint)InteropKernel32.ShareModes.FILE_SHARE_READ,
                                                      IntPtr.Zero,
                                                      (uint)InteropKernel32.CreationDispositions.OPEN_EXISTING,
                                                      (uint)InteropKernel32.FileAttributeFlags.FILE_FLAG_OVERLAPPED,
                                                      IntPtr.Zero);

            // open a handle for reading
            _readHandle = InteropKernel32.CreateFile(_devicePath,
                                                     (uint)InteropKernel32.AccessRights.GENERIC_READ,
                                                     (uint)InteropKernel32.ShareModes.FILE_SHARE_WRITE |
                                                     (uint)InteropKernel32.ShareModes.FILE_SHARE_READ,
                                                     IntPtr.Zero,
                                                     (uint)InteropKernel32.CreationDispositions.OPEN_EXISTING,
                                                     (uint)InteropKernel32.FileAttributeFlags.FILE_FLAG_OVERLAPPED,
                                                     IntPtr.Zero);

            if (_writeHandle == null || _readHandle == null || _writeHandle.IsInvalid || _readHandle.IsInvalid)
            {
                CleanupHandles();

                throw new Exception("Oh noes Cap'n, something is wrong with the handles!");
            }
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void WriteControl(uint controlCode, byte[] writeBuffer)
        {
            throw new NotImplementedException();
        }

        public void WriteControlOverlapped(uint controlCode, byte[] writeBuffer)
        {
            throw new NotImplementedException();
        }

        public void WriteClear(uint controlCode)
        {
            throw new NotImplementedException();
        }

        public void ReadControl(uint controlCode, byte[] writeBuffer, byte[] readBuffer)
        {
            throw new NotImplementedException();
        }

        public void ReadControlOverlapped(uint controlCode, byte[] writeBuffer, byte[] readBuffer)
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] writeBuffer)
        {
            throw new NotImplementedException();
        }

        public void WriteOverlapped(byte[] writeBuffer)
        {
            throw new NotImplementedException();
        }

        public void Read(byte[] readBuffer)
        {
            throw new NotImplementedException();
        }

        public void ReadOverlapped(byte[] readBuffer)
        {
            throw new NotImplementedException();
        }
    }
}
