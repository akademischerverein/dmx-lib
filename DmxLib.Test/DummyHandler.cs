using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DmxLib.Test
{
    public class DummyHandler : IHandler
    {
        static internal DeviceProperty testProp = DeviceProperty.RegisterProperty("unitTestDummy", typeof(uint), 0);
        //internal DeviceProperty testProp2 = DeviceProperty.RegisterProperty("unitTestDummy2", typeof(uint), 0);
        
        public void Update(Device device, ReadOnlyDictionary<DeviceProperty, object> properties, byte[] values) {}

        public bool IsValidValue(DeviceProperty property, object o)
        {
            if (property == testProp)
            {
                return ((uint) o) % 2 == 0;
            }
            throw new ArgumentException();
        }

        public IEnumerable<object> ValidValues(DeviceProperty property)
        {
            if (property == testProp)
            {
                return new object[]
                {
                    (uint) 0, (uint) 2, (uint) 4, (uint) 6
                };
            }
            throw new ArgumentException();
        }

        public ReadOnlyCollection<DeviceProperty> SupportedProperties => new ReadOnlyCollection<DeviceProperty>(new DeviceProperty[]{testProp});
    }
}