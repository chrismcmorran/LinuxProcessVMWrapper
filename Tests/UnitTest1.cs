using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LinuxProcessVMWrapper;
using NUnit.Framework;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestRead()
    {
        var process = Process.GetCurrentProcess();
        int firstBaseValue = process.Read<int>(process.MainModule.BaseAddress);
    }
    
    [Test]
    public void TestWrite()
    {
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
    }
}