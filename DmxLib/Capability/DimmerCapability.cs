using System;
using System.Collections.Generic;
using System.Text;
using DmxLib.StatePart;

namespace DmxLib.Capability
{
    public class DimmerCapability : ICapability
    {
        public uint ChannelOffset { get; init; }

        public uint ChannelCount => 1;

        IEnumerable<Type> ICapability.RequiredStates => [typeof(BrightnessState)];
    }
}
