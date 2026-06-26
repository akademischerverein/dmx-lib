using System;
using System.Collections.Generic;
using System.Text;

namespace DmxLib.Capability
{
    public interface ICapability
    {
        public uint ChannelOffset { get; }

        public uint ChannelCount { get; }

        internal IEnumerable<Type> RequiredStates { get; }
    }
}
