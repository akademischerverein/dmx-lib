using System;
using System.Collections.Generic;
using System.Text;

namespace DmxLib.Rendering
{
    public sealed class RenderContext
    {
        public FixtureDefinition Definition { get; init; }

        public FixtureState State { get; init; }

        public Memory<byte> Buffer { get; init; }

        public ushort StartAddress { get; init; }
    }
}
