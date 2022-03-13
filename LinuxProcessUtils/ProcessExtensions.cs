using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LinuxProcessUtils;

public static class ProcessExtensions
{

    public static unsafe byte[] Read(this Process process, IntPtr address, int length)
    {
        var size = length;
        var ptr = stackalloc byte[size];
        var localIo = new iovec
        {
            iov_base = ptr,
            iov_len = size
        };
        var remoteIo = new iovec
        {
            iov_base = address.ToPointer(),
            iov_len = size
        };

        var res = LinuxLibCImports.process_vm_readv(process.Id, &localIo, 2, &remoteIo, 1, 0);

        if (res < 0)
        {
            throw new Exception($"Failed to read from {process.Id} at address {address} until {address}+{length}");
        }
        
        var result = new byte[length];
        Marshal.Copy((IntPtr)ptr, result, 0, length);
        return result;
    }


    public static unsafe void Write(this Process process, void* address, byte[] value)
    {
        Write(process, (IntPtr)address, value);
    }
    public static unsafe void Write(this Process process, int address, byte[] value)
    {
        Write(process, (IntPtr)address, value);
    }
    public static unsafe void Write(this Process process, IntPtr address, byte[] value)
    {
        int size = Marshal.SizeOf(value[0]) * value.Length;
        IntPtr pnt = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.Copy(value, 0, pnt, value.Length);
            iovec localIo;
            localIo.iov_base = pnt.ToPointer();
            localIo.iov_len = value.Length;

            iovec remoteIo;
            remoteIo.iov_base = address.ToPointer();
            remoteIo.iov_len = size;
            var res = LinuxLibCImports.process_vm_writev(process.Id, &localIo, 1, &remoteIo, 1, 0);
            if (res < 0)
            {
                throw new Exception("Write failed to " + process.Id +
                                    $" at address 0x{address.ToString("x8")} through 0x{address.ToString("x8")}+0x{value.Length.ToString("x8")}");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(pnt);
        }
    }
}