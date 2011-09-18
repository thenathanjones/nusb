using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace NUSB.Interop
{
    /// <summary>
    /// Contains the Windows interop functions related to hid.dll
    /// </summary>
    internal sealed class InteropHID
    {
        [DllImport("hid.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        unsafe internal static extern Boolean HidD_SetFeature(SafeHandle handle, byte* lpReportBuffer, uint nReportBufferSize);

    }
}
