using System.Collections.Generic;
using DmxLib;
using DmxLib.Capability;

namespace DmxLib.Test.Builders
{
    internal sealed class FixtureBuilder
    {
        private readonly List<ICapability> _capabilities = [];
        private ushort _address = 1;
        private uint _nextChannelOffset = 0;

        public FixtureBuilder Rgb()
        {
            _capabilities.Add(new ColorCapability
            {
                ChannelOffset = _nextChannelOffset,
                Emitters =
                [
                    ColorCapability.EmitterColor.Red,
                    ColorCapability.EmitterColor.Green,
                    ColorCapability.EmitterColor.Blue,
                ],
            });
            _nextChannelOffset += 3;
            return this;
        }

        public FixtureBuilder Dimmer()
        {
            _capabilities.Add(new DimmerCapability { ChannelOffset = _nextChannelOffset });
            _nextChannelOffset++;
            return this;
        }

        public FixtureBuilder Relay()
        {
            _capabilities.Add(new RelayCapability { ChannelOffset = _nextChannelOffset });
            _nextChannelOffset++;
            return this;
        }

        public FixtureBuilder Address(ushort address)
        {
            _address = address;
            return this;
        }

        public FixtureDefinition BuildDefinition()
        {
            return new FixtureDefinition
            {
                Capabilities = _capabilities.ToArray(),
            };
        }

        public Fixture Build()
        {
            return new Fixture
            {
                Definition = BuildDefinition(),
                Address = _address,
            };
        }
    }
}
