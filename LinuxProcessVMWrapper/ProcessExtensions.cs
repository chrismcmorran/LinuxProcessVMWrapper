using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LinuxProcessVMWrapper;

public static class ProcessExtensions
{

    /// <summary>
    /// Read a number of bytes from the given address to a byte array.
    /// </summary>
    /// <param name="process">The process to read from.</param>
    /// <param name="address">The address to start reading at.</param>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>A byte[] containing the bytes read.</returns>
    /// <exception cref="Exception">Any exception thrown by an error in process_vm_readv.</exception>
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

    /// <summary>
    /// Read a single byte from the process at the given address.
    /// </summary>
    /// <param name="process">The process to read from.</param>
    /// <param name="address">The address to read from.</param>
    /// <returns>A byte[] containing the bytes read.</returns>
    /// <exception cref="Exception">Any exception thrown by an error in process_vm_readv.</exception>
    public static unsafe byte Read(this Process process, IntPtr address)
    {
        return Read(process, address, 1)[0];
    }
    
    /// <summary>
    /// Read a single byte from the process at the given address.
    /// </summary>
    /// <param name="process">The process to read from.</param>
    /// <param name="address">The address to read from.</param>
    /// <returns>A byte[] containing the bytes read.</returns>
    /// <exception cref="Exception">Any exception thrown by an error in process_vm_readv.</exception>
    public static unsafe byte Read(this Process process, int address)
    {
        return Read(process, (IntPtr)address, 1)[0];
    }
    
    /// <summary>
    /// Read a single byte from the process at the given address.
    /// </summary>
    /// <param name="process">The process to read from.</param>
    /// <param name="address">The address to read from.</param>
    /// <returns>A byte[] containing the bytes read.</returns>
    /// <exception cref="Exception">Any exception thrown by an error in process_vm_readv.</exception>
    public static unsafe byte Read(this Process process, char* address)
    {
        return Read(process, (IntPtr)address, 1)[0];
    }
    
    /// <summary>
    /// Read a single byte from the process at the given address.
    /// </summary>
    /// <param name="process">The process to read from.</param>
    /// <param name="address">The address to read from.</param>
    /// <returns>A byte[] containing the bytes read.</returns>
    /// <exception cref="Exception">Any exception thrown by an error in process_vm_readv.</exception>
    public static unsafe byte Read(this Process process, void* address)
    {
        return Read(process, (IntPtr)address, 1)[0];
    }
    
    
    /// <summary>
    /// Write a byte array to a process at the given address.
    /// </summary>
    /// <param name="process">The process to write to.</param>
    /// <param name="address">The address to start writing at.</param>
    /// <param name="value">The byte array to write.</param>
    /// <exception cref="Exception">Any error that occurs in process_vm_write.</exception>
    public static unsafe void Write(this Process process, void* address, byte value)
    {
        Write(process, (IntPtr)address, new[]{value});
    }
    
    /// <summary>
    /// Write a byte array to a process at the given address.
    /// </summary>
    /// <param name="process">The process to write to.</param>
    /// <param name="address">The address to start writing at.</param>
    /// <param name="value">The byte array to write.</param>
    /// <exception cref="Exception">Any error that occurs in process_vm_write.</exception>
    public static unsafe void Write(this Process process, IntPtr address, byte value)
    {
        Write(process, address, new []{value});
    }

    /// <summary>
    /// Write a byte array to a process at the given address.
    /// </summary>
    /// <param name="process">The process to write to.</param>
    /// <param name="address">The address to start writing at.</param>
    /// <param name="value">The byte array to write.</param>
    /// <exception cref="Exception">Any error that occurs in process_vm_write.</exception>
    public static unsafe void Write(this Process process, void* address, byte[] value)
    {
        Write(process, (IntPtr)address, value);
    }
    
    /// <summary>
    /// Write a byte array to a process at the given address.
    /// </summary>
    /// <param name="process">The process to write to.</param>
    /// <param name="address">The address to start writing at.</param>
    /// <param name="value">The byte array to write.</param>
    /// <exception cref="Exception">Any error that occurs in process_vm_write.</exception>
    public static unsafe void Write(this Process process, int address, byte[] value)
    {
        Write(process, (IntPtr)address, value);
    }
    
    /// <summary>
    /// Write a byte array to a process at the given address.
    /// </summary>
    /// <param name="process">The process to write to.</param>
    /// <param name="address">The address to start writing at.</param>
    /// <param name="value">The byte array to write.</param>
    /// <exception cref="Exception">Any error that occurs in process_vm_write.</exception>
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