using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DmxLib.Test
{
    [TestClass]
    public class DeviceTestsID
    {
        [TestMethod]
        public void TestCorrect()
        {
            var dev1 = new Device("Device 1", 1, 5, new IHandler[]{new DummyHandler()});
            Assert.AreEqual("Device 1", dev1.Name);
            Assert.AreEqual(5u, dev1.Channel);
            Assert.AreEqual(1u, dev1.Width);
            Assert.AreEqual(1, dev1.SupportedProperties.Count);
            Assert.IsTrue(dev1.SupportedProperties.Contains(DummyHandler.testProp));
            CollectionAssert.AreEquivalent(new object[]{(uint)0, (uint)2, (uint)4, (uint)6}, dev1.ValidValues(DummyHandler.testProp).ToArray());
            
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Width must be greater than 0")]
        public void TestIncorrectWidth()
        {
            var dev1 = new Device("Device 1", 0, 1, new IHandler[0]);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Channel must be greater than 0")]
        public void TestInvalidChannel()
        {
            var dev1 = new Device("Device 1", 1, 0, new IHandler[0]);
        }
    }
}