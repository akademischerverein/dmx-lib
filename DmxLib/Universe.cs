using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DmxLib
{
    public class Universe
    {
        private HashSet<Device> devices = new HashSet<Device>();
        //groups
        private ISink sink;
        private byte[] values;
        
        public Universe(uint numChannels, ISink sink)
        {
            Size = numChannels;
            this.sink = sink;
            values = new byte[numChannels];
            //scene
            //hooks
        }

        public uint Size { get; }
        public HashSet<Device> Devices => devices;

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

            if (device.universe != null)
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
            device.universe = this;
            ApplyProperties(device, device._properties);
        }

        public void RemoveDevice(Device device)
        {
            if (device.universe == this)
            {
                device.universe = null;
            }

            devices.Remove(device);
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

            //group props

            var handleProps = new ReadOnlyDictionary<DeviceProperty, object>(props);
            foreach (var h in device.handlers)
            {
                h.Update(device, handleProps, devValues);
            }

            for (uint i = 0; i < device.Width; i++)
            {
                values[i + device.Channel - 1] = devValues[i];
            }
            //hooks
            sink.Update(this, values);
        }
    }
}