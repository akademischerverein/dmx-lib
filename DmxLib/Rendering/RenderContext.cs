using System;
using System.Collections.Generic;
using System.Text;

namespace DmxLib.Rendering
{
    public sealed class RenderContext
    {
        public FixtureDefinition Definition { get; }

        public FixtureState State { get; }

        public Memory<byte> Buffer { get; }

        public ushort StartAddress { get; }
    }
}
