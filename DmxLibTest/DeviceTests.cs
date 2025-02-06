using System;
using DmxLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DmxLibTest
{
    [TestClass]
    public class DeviceTests
    {
        [TestMethod]
        public void TestCorrectWidth()
        {
            var dev1 = new Device("Device 1", 1, 1, new IHandler[0]);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Width must be greater than 0")]
        public void TestIncorrectWidth()
        {
            var dev1 = new Device("Device 1", 0, 1, new IHandler[0]);
        }
    }
}