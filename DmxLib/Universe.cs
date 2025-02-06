using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DmxLib.Util;

namespace DmxLib
{
    public class Universe
    {
        private readonly ISink _sink;
        private byte[] _values;
        private readonly Dictionary<uint, IDevice> _channels = new Dictionary<uint, IDevice>();

        public Universe(uint numChannels, ISink sink)
        {
            Size = numChannels;
            _sink = sink;
            _values = new byte[numChannels];
            Devices = new HashSet<IDevice>();
            Hooks = _dummyApply;
            //todo scene
        }

        public uint Size { get; }
        public HashSet<IDevice> Devices { get; }

        public delegate void ApplyEvent(Universe universe, ref byte[] values);

        public delegate void ApplyPropertiesDelegate(IDevice device, byte[] values);

        public ApplyEvent Hooks;

        private static void _dummyApply(Universe universe, ref byte[] values) {}

        public void AddDevice(IDevice device)
        {
            if (Devices.Contains(device) || Devices.SelectMany(d => d.AllChildren).Contains(device))
            {
                throw new ArgumentException("Device already part of this universe", nameof(device));
            }

            if (device.Channels.Any(ch => ch > Size))
            {
                throw new ArgumentException("Device doesn't fit in this universe");
            }

            if (device.ApplyEvent != null)
            {
                throw new ArgumentException("Device already belongs to a universe");
            }

            if (device.Channels.Any(ch => _channels.ContainsKey(ch)))
            {
                throw new ArgumentException("Channel conflict detected");
            }

            foreach (var ch in device.Channels)
            {
                _channels[ch] = device;
            }

            Devices.Add(device);
            device.ApplyEvent = ApplyProperties;
            //todo: apply properties on addition
        }

        public void RemoveDevice(IDevice device)
        {
            if (!Devices.Contains(device)) return;
            Devices.Remove(device);
            foreach (var ch in device.Channels)
            {
                _channels.Remove(ch);
            }

            device.ApplyEvent = null;
        }

        public IDevice GetDeviceByChannel(uint channel)
        {
            return _channels.ContainsKey(channel) ? _channels[channel] : null;
        }

        private void ApplyProperties(IDevice device, byte[] deviceValues)
        {
            if (!Devices.Contains(device))
            {
                throw new ArgumentException("Specified device not in universe", nameof(device));
            }

            var channels = device.Channels.ToArray();

            for (uint i = 0; i < deviceValues.Length; i++)
            {
                _values[channels[i] - 1] = deviceValues[i];
            }

            Hooks?.Invoke(this, ref _values);
            _sink.Update(this, _values);
        }
    }
}