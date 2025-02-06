using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DmxLib
{
    public interface IDevice
    {
        object Get(DeviceProperty property);
        void Set(DeviceProperty property, object value);
        ReadOnlyCollection<object> ValidValues(DeviceProperty property);
        bool IsValidValue(DeviceProperty property, object o);
        ReadOnlyDictionary<uint, byte> ApplyProperties(ReadOnlyDictionary<DeviceProperty, object> properties);
        ReadOnlyCollection<DeviceProperty> SupportedProperties { get; }
        string Name { get; }
        ReadOnlyCollection<IDevice> Children { get; }
        ReadOnlyCollection<IDevice> AllChildren { get; }
        ReadOnlyCollection<uint> Channels { get; }
        Universe.ApplyPropertiesDelegate ApplyEvent { get; set; }
    }
}