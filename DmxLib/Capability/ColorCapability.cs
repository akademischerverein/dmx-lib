using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DmxLib.StatePart;

namespace DmxLib.Capability
{
    public class ColorCapability : ICapability
    {
        public uint ChannelOffset { get; init; }

        public uint ChannelCount => (uint)Emitters.Count;

        public IReadOnlyList<EmitterColor> Emitters { get; init; } = [];

        IEnumerable<Type> ICapability.RequiredStates => [typeof(BrightnessState), typeof(ColorState)];

        public enum EmitterColor
        {
            Red,
            Green,
            Blue,
            White
        }
    }
}
