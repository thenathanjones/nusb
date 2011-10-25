using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using NUSB.Interop;

namespace NUSB.Controller
{
    /// <summary>
    /// USB Controller that uses the Windows API to interact with USB devices
    /// </summary>
    public sealed class Windows32USBController : IUSBController
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
            CleanupHandles();
        }

        public unsafe void WriteControl(uint controlCode, byte[] writeBuffer)
        {
            uint bytesReturned = 0;

            fixed (byte* inBuffer = writeBuffer)
            {
                var success = false;
                try
                {
                    success = InteropKernel32.DeviceIoControl(_writeHandle,
                                                              controlCode,
                                                              inBuffer,
                                                              writeBuffer == null ? 0 : (uint)writeBuffer.Length,
                                                              null,
                                                              0,
                                                              ref bytesReturned,
                                                              null);
                }
                catch (ObjectDisposedException)
                {
                    throw new Exception("File handle already closed");
                }

                // retrieve the error on failures
                if (!success) { HandleIOError(false); }
            }
        }

        private void HandleIOError(bool ignoreOverlapped)
        {
            var lastWin32Error = Marshal.GetLastWin32Error();

            // if the error is something other than pending IO (i.e. because we're using overlapped)
            if (!ignoreOverlapped || lastWin32Error != (int)InteropCommon.Win32Errors.ERROR_IO_PENDING)
            {
                throw new Exception("Unknown Win32 Error occurred: " + lastWin32Error);
            }
        }

        public unsafe void WriteControlOverlapped(uint controlCode, byte[] writeBuffer)
        {
            var completedEvent = new ManualResetEvent(false);
            uint bytesReturned = 0;
            var outOverlapped = new Overlapped();
            outOverlapped.EventHandleIntPtr = completedEvent.SafeWaitHandle.DangerousGetHandle();
            NativeOverlapped* outNativeOverlapped = outOverlapped.Pack(null, null);

            try
            {
                fixed (byte* inBuffer = writeBuffer)
                {
                    var success = false;
                    try
                    {
                        success = InteropKernel32.DeviceIoControl(_writeHandle,
                                                                  controlCode,
                                                                  inBuffer,
                                                                  writeBuffer == null ? 0 : (uint)writeBuffer.Length,
                                                                  null,
                                                                  0,
                                                                  ref bytesReturned,
                                                                  outNativeOverlapped);
                    }
                    catch (ObjectDisposedException)
                    {
                        throw new Exception("File handle already closed");
                    }

                    if (!success)
                    {
                        HandleIOError(true);

                        CancelOverlapped(_writeHandle, completedEvent);
                    }
                }
            }
            finally
            {
                Overlapped.Free(outNativeOverlapped);
            }
        }

        private void CancelOverlapped(SafeFileHandle handle, ManualResetEvent completedEvent)
        {
            lock (_cancelIOLock)
            {
                if (!completedEvent.WaitOne(OVERLAPPED_TIMEOUT) && handle != null && !handle.IsClosed && !handle.IsInvalid)
                    InteropKernel32.CancelIo(handle);
            }
        }

        public void WriteClear(uint controlCode)
        {
            // cancel pending operations
            lock (_cancelIOLock)
            {
                InteropKernel32.CancelIo(_readHandle);
                InteropKernel32.CancelIo(_writeHandle);
            }

            WriteControl(controlCode, null);
        }

        public unsafe void ReadControl(uint controlCode, byte[] writeBuffer, byte[] readBuffer)
        {
            uint bytesReturned = 0;

            fixed (byte* inBuffer = writeBuffer)
            {
                fixed (byte* outBuffer = readBuffer)
                {
                    var success = false;
                    try
                    {
                        success = InteropKernel32.DeviceIoControl(_readHandle,
                                                                  controlCode,
                                                                  inBuffer,
                                                                  writeBuffer == null ? 0 : (uint)writeBuffer.Length,
                                                                  outBuffer,
                                                                  readBuffer == null ? 0 : (uint)readBuffer.Length,
                                                                  ref bytesReturned,
                                                                  null);
                    }
                    catch (ObjectDisposedException)
                    {
                        throw new Exception("File handle already closed");
                    }

                    if (!success) { HandleIOError(false); }
                }
            }
        }

        public unsafe void ReadControlOverlapped(uint controlCode, byte[] writeBuffer, byte[] readBuffer)
        {
            var completedEvent = new ManualResetEvent(false);
            uint bytesReturned = 0;
            var inOverlapped = new Overlapped();
            inOverlapped.EventHandleIntPtr = completedEvent.SafeWaitHandle.DangerousGetHandle();
            NativeOverlapped* inNativeOverlapped = inOverlapped.Pack(null, null);

            try
            {
                fixed (byte* inBuffer = writeBuffer)
                {
                    fixed (byte* outBuffer = readBuffer)
                    {
                        var success = false;
                        try
                        {
                            success = InteropKernel32.DeviceIoControl(_readHandle,
                                                                      controlCode,
                                                                      inBuffer,
                                                                      writeBuffer == null ? 0 : (uint) writeBuffer.Length,
                                                                      outBuffer,
                                                                      readBuffer == null ? 0 : (uint) readBuffer.Length,
                                                                      ref bytesReturned,
                                                                      inNativeOverlapped);
                        }
                        catch (ObjectDisposedException)
                        {
                            throw new Exception("File handle already closed");
                        }

                        if (!success)
                        {
                            HandleIOError(true);

                            CancelOverlapped(_readHandle, completedEvent);
                        }
                    }
                }
            }
            finally
            {
                Overlapped.Free(inNativeOverlapped);
            }
        }

        public unsafe void Write(byte[] writeBuffer)
        {
            uint bytesWritten = 0;

            fixed (byte* inBuffer = writeBuffer)
            {
                var success = false;
                try
                {
                    success = InteropKernel32.WriteFile(_writeHandle,
                                                        inBuffer,
                                                        (uint)writeBuffer.Length,
                                                        ref bytesWritten,
                                                        null);
                }
                catch (ObjectDisposedException)
                {
                    throw new Exception("File handle already closed");
                }

                if (!success) { HandleIOError(false); }
            }
        }

        public unsafe void WriteOverlapped(byte[] writeBuffer)
        {
            var completedEvent = new ManualResetEvent(false);
            uint bytesWritten = 0;
            var outOverlapped = new Overlapped();
            outOverlapped.EventHandleIntPtr = completedEvent.SafeWaitHandle.DangerousGetHandle();
            NativeOverlapped* outNativeOverlapped = outOverlapped.Pack(null, null);

            try
            {
                // send the data to the device
                fixed (byte* inBuffer = writeBuffer)
                {
                    var success = false;
                    try
                    {
                        success = InteropKernel32.WriteFile(_writeHandle,
                                                            inBuffer,
                                                            (uint)writeBuffer.Length,
                                                            ref bytesWritten,
                                                            outNativeOverlapped);
                    }
                    catch (ObjectDisposedException)
                    {
                        throw new Exception("File handle already closed");
                    }

                    if (!success)
                    {
                        HandleIOError(true);

                        CancelOverlapped(_writeHandle, completedEvent);
                    }
                }
            }
            finally
            {
                Overlapped.Free(outNativeOverlapped);
            }
        }

        public unsafe void Read(byte[] readBuffer)
        {
            uint bytesRead = 0;

            fixed (byte* outBuffer = readBuffer)
            {
                var success = false;
                try
                {
                    success = InteropKernel32.ReadFile(_readHandle,
                                                       outBuffer,
                                                       (uint)readBuffer.Length,
                                                       ref bytesRead,
                                                       null);
                }
                catch (ObjectDisposedException)
                {
                    throw new Exception("File handle already closed");
                }

                if (!success)
                {
                    HandleIOError(false);
                }
            }
        }

        public unsafe void ReadOverlapped(byte[] readBuffer)
        {
            var completedEvent = new ManualResetEvent(false);
            uint bytesRead = 0;
            var inOverlapped = new Overlapped();
            inOverlapped.EventHandleIntPtr = completedEvent.SafeWaitHandle.DangerousGetHandle();
            NativeOverlapped* inNativeOverlapped = inOverlapped.Pack(null, null);

            try
            {
                // send the data to the device
                fixed (byte* outBuffer = readBuffer)
                {
                    var success = false;
                    try
                    {
                        success = InteropKernel32.ReadFile(_readHandle,
                                                           outBuffer,
                                                           (uint)readBuffer.Length,
                                                           ref bytesRead,
                                                           inNativeOverlapped);
                    }
                    catch (ObjectDisposedException)
                    {
                        throw new Exception("File handle already closed");
                    }

                    if (!success)
                    {
                        HandleIOError(true);

                        CancelOverlapped(_readHandle, completedEvent);
                    }
                }
            }
            finally
            {
                // clean up
                Overlapped.Free(inNativeOverlapped);
            }
        }

        public unsafe void HidSetFeature(byte[] reportBuffer)
        {
            fixed (byte* inBuffer = reportBuffer)
            {
                var success = false;
                try
                {
                    success = InteropHID.HidD_SetFeature(_writeHandle,
                                                         inBuffer,
                                                         (uint)reportBuffer.Length);
                }
                catch (ObjectDisposedException)
                {
                    throw new Exception("File handle already closed");
                }

                if (!success) { HandleIOError(false); }
            }
        }

        private const int OVERLAPPED_TIMEOUT = 2000;
    }
}
