using System;
using System.Collections.Generic;
using System.Text;

namespace DmxLib
{
    public sealed class Fixture
    {
        public required FixtureDefinition Definition { get; init; }

        public required ushort Address { get; init; }
    }
}
