using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DmxLib.Testing
{
    public class DimmerHandler : IHandler
    {
        public void Update(Device device, ReadOnlyDictionary<DeviceProperty, object> properties, Dictionary<uint, byte> values)
        {
            values[device.Channels[0]] = (byte) (255 * (double)properties[Program.PropertyDimming]);
        }

        public bool IsValidValue(DeviceProperty property, object o)
        {
            return o is double;
        }

        public ReadOnlyCollection<object> ValidValues(DeviceProperty property)
        {
            throw new System.ArgumentException();
        }

        public ReadOnlyCollection<DeviceProperty> SupportedProperties => new ReadOnlyCollection<DeviceProperty>(new[]{Program.PropertyDimming});
    }
}