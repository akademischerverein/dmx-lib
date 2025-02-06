using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DmxLib.Util;

namespace DmxLib
{
    public class Universe
    {
        private HashSet<Device> devices = new HashSet<Device>();
        private HashSet<DeviceGroup> groups = new HashSet<DeviceGroup>();
        //groups
        private ISink sink;
        private byte[] values;
        
        public Universe(uint numChannels, ISink sink)
        {
            Size = numChannels;
            this.sink = sink;
            values = new byte[numChannels];
            //todo scene
            //todo hooks
        }

        public uint Size { get; }
        public HashSet<Device> Devices => devices;
        public HashSet<DeviceGroup> Groups => groups;

        public void AddDevice(Device device)
        {
            if (devices.Contains(device))
            {
                throw new ArgumentException("Device already part of this universe", nameof(device));
            }

            if ((device.Channel + device.Width) > Size)
            {
                throw new ArgumentException("Device doesn't fit in this universe");
            }

            if (device.Universe != null)
            {
                throw new ArgumentException("Device already belongs to a universe");
            }
            
            var occupiedChs = new HashSet<uint>();
            foreach (var dev in devices)
            {
                for (var i = dev.Channel; i < dev.Channel+dev.Width; i++)
                {
                    occupiedChs.Add(i);
                }
            }

            for (var i = device.Channel; i < device.Channel + device.Width; i++)
            {
                if (occupiedChs.Contains(i))
                {
                    throw new ArgumentException(string.Format("Channel %i is already occupied", i));
                }
            }

            devices.Add(device);
            device.Universe = this;
            ApplyProperties(device, device.Properties);
        }

        public void AddGroup(DeviceGroup group)
        {
            if (groups.Contains(group))
            {
                throw new ArgumentException("Group already part of this universe", nameof(group));
            }
            
            if (group.Universe != null)
            {
                throw new ArgumentException("Group already belongs to a universe");
            }

            if (group.Devices.Select(d => d.Group).Any(g => g != null))
            {
                throw new ArgumentException("No device must have an active group");
            }

            foreach (var dev in group.Devices)
            {
                dev.Group = group;
            }

            group.Universe = this;
            groups.Add(group);
        }

        public void RemoveDevice(Device device)
        {
            if (device.Universe == this)
            {
                device.Universe = null;
            }

            devices.Remove(device);
        }

        public void RemoveGroup(DeviceGroup group)
        {
            if (group.Universe == this)
            {
                group.Universe = null;
                foreach (var dev in group.Devices)
                {
                    dev.Group = null;
                }
            }

            groups.Remove(group);
        }

        public Device GetDeviceByChannel(uint channel)
        {
            foreach (var dev in devices)
            {
                for (var i = dev.Channel; i < dev.Channel + dev.Width; i++)
                {
                    if (i == channel)
                    {
                        return dev;
                    }
                }
            }

            return null;
        }

        public void ApplyProperties(Device device, Dictionary<DeviceProperty, object> properties)
        {
            if (!devices.Contains(device))
            {
                throw new ArgumentException("Specified device not in universe", nameof(device));
            }

            var devValues = new byte[device.Width];
            var props = new Dictionary<DeviceProperty, object>(properties);

            if (device.Group != null)
            {
                foreach (var prop in props.Keys)
                {
                    if (prop.Type == typeof(Color))
                    {
                        var c = (Color)device.Group.Properties[prop];

                        if (c.R != 0.0 || c.G != 0.0 || c.B != 0.0)
                        {
                            props[prop] = c;
                        }
                    } else if (prop.Type == typeof(Vector2f))
                    {
                        var gPos = (Vector2f)device.Group.Properties[prop];
                        var dPos = (Vector2f)device.Properties[prop];
                        props[prop] = new Vector2f(gPos.X + dPos.X, gPos.Y + dPos.Y);
                    } else if (prop.Type == typeof(double) || prop.Type == typeof(float))
                    {
                        props[prop] = (double)props[prop] * (double)device.Group.Properties[prop];
                    } else if (prop.Type == typeof(bool))
                    {
                        props[prop] = (bool) props[prop] || (bool) device.Group.Properties[prop];
                    }
                    else
                    {
                        if (device.Group.Properties[prop] != null)
                        {
                            props[prop] = device.Group.Properties[prop];
                        }
                    }
                }
            }

            var handleProps = new ReadOnlyDictionary<DeviceProperty, object>(props);
            foreach (var h in device.Handlers)
            {
                h.Update(device, handleProps, devValues);
            }

            for (uint i = 0; i < device.Width; i++)
            {
                values[i + device.Channel - 1] = devValues[i];
            }
            //todo hooks
            sink.Update(this, values);
        }
    }
}