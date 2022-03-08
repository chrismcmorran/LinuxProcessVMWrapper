using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LinuxProcessUtils;

public static class ProcessExtensions
{

    public static unsafe bool Read<T>(this Process process, IntPtr address, out T value) where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
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

        var res = LinuxLibCImports.process_vm_readv(process.Id, &localIo, 1, &remoteIo, 1, 0);
        value = *(T*) ptr;
        return res != -1;
    }
    
    public static unsafe T[] Read<T>(this Process process, IntPtr address, int length = 1) where T : unmanaged
    {
        var value = new T[length];
        for (int i = 0; i < length; i++)
        {
            var pointer = new IntPtr(address.ToInt64() + i);
            Read<T>(process, pointer, out value[i]);
        }

        return value;
    }

    
    public static unsafe bool Write<T>(Process process, T value, IntPtr address) where T : unmanaged
    {
        var ptr = &value;
        var size = Unsafe.SizeOf<T>();
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
        var res = LinuxLibCImports.process_vm_writev(process.Id, &localIo, 1, &remoteIo, 1, 0);
        return res != -1;
    }
}