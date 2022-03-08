using System.Runtime.InteropServices;

namespace LinuxProcessUtils;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct iovec
{
    public void* iov_base;
    public int iov_len;
}
public static class LinuxLibCImports
{
    [DllImport("libc")]
    public static extern unsafe int process_vm_writev(int pid,
        iovec* local_iov,
        ulong liovcnt,
        iovec* remote_iov,
        ulong riovcnt,
        ulong flags);
        
    [DllImport("libc")]
    public static extern unsafe int process_vm_readv(int pid,
        iovec* local_iov,
        ulong liovcnt,
        iovec* remote_iov,
        ulong riovcnt,
        ulong flags);
}