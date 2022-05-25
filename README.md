* About
This is a C# based package for reading and writing to the memory of an external process on modern Linux based systems. This package provides a set of functions which wrap around the `process_vm_readv` and `process_vm_writev` system calls. These functions are provided as extension methods to the System.Diagnostics.Process class.

* Examples
** Reading
```
var process = Process.GetCurrentProcess();
int firstBaseValue = process.Read<int>(process.MainModule.BaseAddress);
```
** Writing
```
var process = Process.GetCurrentProcess();
int obj = new int();
GCHandle handle = GCHandle.Alloc(obj, GCHandleType.Weak);
try
{
    // Get a pointer to the int object we allocated in this process.
    IntPtr pointer = GCHandle.ToIntPtr(handle);

    // Write the value 7 to the address the pointer points to.
    process.Write<int>(pointer, 7);

    // Read the address the pointer points to and ensure it's 7.
    Assert.AreEqual(7, process.Read<int>(pointer));
}
finally
{
    handle.Free();
}	
```
