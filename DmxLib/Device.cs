using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using DmxLib.Util;

namespace DmxLib
{
    public class Device : IDevice
    {
        private readonly IHandler[] _handlers;
        private readonly Dictionary<DeviceProperty, object> _properties;
        private readonly List<uint> _channels;

        public Device(string name, uint width, uint channel, IHandler[] handlers)
        {
            if (width < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than 0");
            }

            if (channel < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(channel), "Channel must be greater than 0");
            }

            Name = name;
            _channels = new List<uint>();
            for (var i = channel; i < channel + width; i++)
            {
                _channels.Add(i);
            }
            _properties = new Dictionary<DeviceProperty, object>();
            _handlers = handlers;

            _properties = handlers.SelectMany(h => h.SupportedProperties).Distinct().ToDictionary(p => p, p => p.DefaultValue);
        }

        public ReadOnlyDictionary<uint, byte> ApplyProperties(ReadOnlyDictionary<DeviceProperty, object> properties)
        {
            var values = new Dictionary<uint, byte>();
            foreach (var ch in Channels)
            {
                values[ch] = 0;
            }
            foreach (var h in _handlers)
            {
                h.Update(this, properties, values);
            }

            return new ReadOnlyDictionary<uint, byte>(values);
        }

        public ReadOnlyCollection<DeviceProperty> SupportedProperties => new ReadOnlyCollection<DeviceProperty>(_properties.Keys.ToList());
        public string Name { get; }
        public ReadOnlyCollection<IDevice> Children => new ReadOnlyCollection<IDevice>(new List<IDevice>());
        public ReadOnlyCollection<IDevice> AllChildren => new ReadOnlyCollection<IDevice>(new List<IDevice>());
        public ReadOnlyCollection<uint> Channels => new ReadOnlyCollection<uint>(_channels);

        public Universe.ApplyPropertiesDelegate ApplyEvent { get; set; }

        public object Get(DeviceProperty property)
        {
            return _properties[property];
        }

        public void Set(DeviceProperty property, object value)
        {
            if (!property.Type.IsInstanceOfType(value))
            {
                throw new ArgumentException("Property value of illegal type", nameof(value));
            }

            if (!_properties.ContainsKey(property))
            {
                throw new ArgumentException("Property not supported for this device", nameof(property));
            }

            if (_handlers.Any(h => h.SupportedProperties.Contains(property) && !h.IsValidValue(property, value)))
            {
                throw new ArgumentException("Property value not accepted by handler", nameof(value));
            }

            _properties[property] = value;
            var values = ApplyProperties(new ReadOnlyDictionary<DeviceProperty, object>(_properties));
            ApplyEvent?.Invoke(this, values);
        }

        public ReadOnlyCollection<object> ValidValues(DeviceProperty property)
        {
            var supported = new HashSet<object>();
            foreach (var h in _handlers)
            {
                if (h.SupportedProperties.Contains(property))
                {
                    supported.UnionWith(h.ValidValues(property));
                }
            }

            return new ReadOnlyCollection<object>(supported.ToList());
        }

        public bool IsValidValue(DeviceProperty property, object o)
        {
            var ret =  _handlers.Where(h => h.SupportedProperties.Contains(property)).All(h => h.IsValidValue(property, o));
            return ret;
        }
    }
}