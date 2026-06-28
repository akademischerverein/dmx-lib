using System.Collections.Generic;
using DmxLib;

namespace DmxLib.Test.Builders
{
    internal sealed class UniverseBuilder
    {
        private readonly List<Fixture> _fixtures = [];
        private int _id = 1;

        public UniverseBuilder Id(int id)
        {
            _id = id;
            return this;
        }

        public UniverseBuilder WithFixture(Fixture fixture)
        {
            _fixtures.Add(fixture);
            return this;
        }

        public Universe Build()
        {
            return new Universe
            {
                Id = _id,
                Fixtures = _fixtures.ToArray(),
            };
        }
    }
}
