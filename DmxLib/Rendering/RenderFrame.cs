using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DmxLib.Rendering
{
    public class RenderFrame : IEnumerable<KeyValuePair<Fixture, FixtureState>>
    {
        internal RenderFrame(Dictionary<Fixture, FixtureState> fixStates)
        {
            states = fixStates;
        }

        private readonly Dictionary<Fixture, FixtureState> states = [];

        public FixtureState Get(Fixture fixture)
        {
            return states[fixture];
        }

        public IEnumerator<KeyValuePair<Fixture, FixtureState>> GetEnumerator()
        {
            return states.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
