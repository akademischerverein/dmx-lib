using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DmxLib.Testing
{
    public class DimmerHandler : IHandler
    {
        private readonly uint _idx;

        public DimmerHandler(uint idx)
        {
            _idx = idx;
        }
        
        public void Update(Device device, ReadOnlyDictionary<DeviceProperty, object> properties, byte[] values)
        {
            values[_idx] = (byte) (255 * (double)properties[Program.PropertyDimming]);
        }

        public bool IsValidValue(DeviceProperty property, object o)
        {
            return o is double;
        }

        public IEnumerable<object> ValidValues(DeviceProperty property)
        {
            throw new System.ArgumentException();
        }

        public ReadOnlyCollection<DeviceProperty> SupportedProperties => new ReadOnlyCollection<DeviceProperty>(new DeviceProperty[]{Program.PropertyDimming});
    }
}