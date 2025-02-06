using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DmxLib
{
    public interface IDevice
    {
        object Get(DeviceProperty property);
        void Set(DeviceProperty property, object value);
        IEnumerable<object> ValidValues(DeviceProperty property);
        bool IsValidValue(DeviceProperty property, object o);
        byte[] ApplyProperties(ReadOnlyDictionary<DeviceProperty, object> properties);
        ReadOnlyCollection<DeviceProperty> SupportedProperties { get; }
        string Name { get; }
        IEnumerable<IDevice> Children { get; }
        IEnumerable<IDevice> AllChildren { get; }
        List<uint> Channels { get; }
        Universe.ApplyPropertiesDelegate ApplyEvent { get; set; }
    }
}