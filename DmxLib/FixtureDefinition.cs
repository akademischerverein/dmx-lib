using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DmxLib.Capability;
using DmxLib.StatePart;

namespace DmxLib
{
    public class FixtureDefinition
    {
        public IReadOnlyList<ICapability> Capabilities { get; init; }

        public uint NumChannels => Capabilities.Max(c => c.ChannelOffset + c.ChannelCount);

        public FixtureState CreateState()
        {
            var state = new FixtureState();
            var requiredStateTypes = Capabilities.SelectMany(cap => cap.RequiredStates).Distinct();
            foreach(var stateType in requiredStateTypes)
            {
                if(!stateType.IsSubclassOf(typeof(FixtureStatePart)))
                {
                    throw new InvalidDataException();
                }
                var constructor = stateType.GetConstructor([]);
                state.States.Add((FixtureStatePart)constructor.Invoke([]));
            }
            return state;
        }
    }
}
