using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LinuxProcessVMWrapper;
using NUnit.Framework;

namespace TestProject1;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void SimpleRead()
    {
        var process = Process.GetCurrentProcess();
        var address = process.MainModule.BaseAddress;
        var read = process.Read(address, 3);
        Assert.AreEqual("\u007fEL", Encoding.ASCII.GetString(read));
    }
    
    [Test]
    public unsafe void SimpleWrite()
    {
        char aChar = 'a';
        var process = Process.GetCurrentProcess();
        var address = &aChar;
        var length = 1;
        var initialValue = process.Read((IntPtr)address, length);
        var toWrite = new byte[] {(byte)'b'};
        process.Write(address, toWrite);
        var readBack = process.Read((IntPtr)address, length);
        Assert.AreEqual(toWrite, readBack);
        process.Write(address, new[]{(byte)aChar});
        Assert.AreEqual(new[]{(byte)aChar}, process.Read((IntPtr)address, length));
        process.Write(address, initialValue);
        Assert.AreEqual(process.Read(address), initialValue[0]);
    }
    
    
}