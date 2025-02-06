using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DmxLib
{
    public class DeviceGroup : IDevice
    {
        private readonly HashSet<IDevice> _devices;
        private readonly Dictionary<DeviceProperty, object> _properties;
        
        public DeviceGroup(string name, IEnumerable<IDevice> devices)
        {
            Name = name;
            _devices = new HashSet<IDevice>(devices);
            AllChildren = _devices.SelectMany(d => d.AllChildren).Union(_devices).Distinct();
            _properties = _devices.SelectMany(d => d.SupportedProperties).Distinct().ToDictionary(p => p, p => p.DefaultValue);
            Channels = _devices.SelectMany(d => d.Channels).ToList();
            if (Channels.Count != Channels.Distinct().Count())
            {
                throw new ArgumentException("Channel can only be assigned to one device");
            }

            foreach (var dev in _devices)
            {
                dev.ApplyEvent = ChildUpdated;
            }
        }
        
        public string Name { get; }
        public IEnumerable<IDevice> Children => new ReadOnlyCollection<IDevice>(_devices.ToList());
        public IEnumerable<IDevice> AllChildren { get; }
        public List<uint> Channels { get; }
        public Universe.ApplyPropertiesDelegate ApplyEvent { get; set; }
        public byte[] ApplyProperties(ReadOnlyDictionary<DeviceProperty, object> properties)
        {
            var values = new byte[Channels.Count];
            uint i = 0;
            foreach (var dev in _devices)
            {
                var props = dev.SupportedProperties.ToDictionary(p => p, p => dev.Get(p));
                foreach (var prop in dev.SupportedProperties)
                {
                    props[prop] = prop.Project(properties[prop], props[prop]);
                    if (!dev.IsValidValue(prop, props[prop]))
                    {
                        throw new Exception("Projected value not accepted by device");
                    }
                }

                var devValues = dev.ApplyProperties(new ReadOnlyDictionary<DeviceProperty, object>(props));
                for (var j = 0; j < devValues.Length; j++)
                {
                    values[i++] = devValues[j];
                }
            }

            return values;
        }

        public ReadOnlyCollection<DeviceProperty> SupportedProperties => new ReadOnlyCollection<DeviceProperty>(_properties.Keys.ToList());
        
        public object Get(DeviceProperty property)
        {
            return _properties[property];
        }

        private void ChildUpdated(IDevice device, byte[] deviceValues)
        {
            var values = ApplyProperties(new ReadOnlyDictionary<DeviceProperty, object>(_properties));
            ApplyEvent(this, values);
        }

        public void Set(DeviceProperty property, object value)
        {
            if (!property.Type.IsInstanceOfType(value))
            {
                throw new ArgumentException("Property value of illegal type", nameof(value));
            }

            if (!_properties.ContainsKey(property))
            {
                throw new ArgumentException("Property not supported for this device group", nameof(property));
            }
            
            if (!IsValidValue(property, value))
            {
                throw new ArgumentException("Property value not accepted by devices", nameof(value));
            }

            _properties[property] = value;
            var values = ApplyProperties(new ReadOnlyDictionary<DeviceProperty, object>(_properties));
            ApplyEvent(this, values);
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