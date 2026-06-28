using System;
using System.Collections.Generic;
using System.Text;
using DmxLib.StatePart;

namespace DmxLib.Capability
{
    public class RelayCapability : ICapability
    {
        public uint ChannelOffset { get; set; }

        public uint ChannelCount => 1;

        IEnumerable<Type> ICapability.RequiredStates => [typeof(RelayState)];
    }
}
