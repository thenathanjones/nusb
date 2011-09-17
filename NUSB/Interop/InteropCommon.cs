using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUSB.Interop
{
    /// <summary>
    /// Contains the Windows interop functions related to devices
    /// </summary>
    internal sealed class InteropCommon
    {
        [Flags]
        public enum Win32Errors
        {
            ERROR_IO_PENDING = 997
        }
    }
}
