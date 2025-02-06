using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DmxLib
{
    public class DeviceGroup //: IDevice
    {
        private readonly HashSet<Device> _devices;
        internal Universe Universe;
        internal readonly Dictionary<DeviceProperty, object> Properties;
        
        public DeviceGroup(string name, IEnumerable<Device> devices)
        {
            Name = name;
            _devices = new HashSet<Device>(devices);

            Properties = _devices.SelectMany(d => d.SupportedProperties).Distinct().ToDictionary(p => p, p => p.DefaultValue);
        }
        
        public readonly string Name;
        public ReadOnlyCollection<Device> Devices => new ReadOnlyCollection<Device>(_devices.ToList());
        
        public ReadOnlyCollection<DeviceProperty> SupportedProperties => new ReadOnlyCollection<DeviceProperty>(Properties.Keys.ToList());
        
        public object Get(DeviceProperty property)
        {
            return Properties[property];
        }

        public void Set(DeviceProperty property, object value)
        {
            if (!property.Type.IsInstanceOfType(value))
            {
                throw new ArgumentException("Property value of illegal type", nameof(value));
            }

            if (!Properties.ContainsKey(property))
            {
                throw new ArgumentException("Property not supported for this device group", nameof(property));
            }
            
            if (!IsValidValue(property, value))
            {
                throw new ArgumentException("Property value not accepted by devices", nameof(value));
            }

            Properties[property] = value;

            foreach (var dev in _devices)
            {
                //Universe.ApplyProperties(dev, dev.Properties);
            }
        }
        
        public IEnumerable<object> ValidValues(DeviceProperty property)
        {
            var supported = new HashSet<object>(_devices.First().ValidValues(property));
            foreach (var d in _devices)
            {
                supported.IntersectWith(d.ValidValues(property));
            }

            return supported;
        }

        public bool IsValidValue(DeviceProperty property, object o)
        {
            return _devices.Where(d => d.SupportedProperties.Contains(property)).All(d => d.IsValidValue(property, o));
        }
    }
}