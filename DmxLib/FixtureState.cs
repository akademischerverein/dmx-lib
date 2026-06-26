using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DmxLib.StatePart;

namespace DmxLib
{
    public sealed class FixtureState
    {
        internal List<FixtureStatePart> States { get; } = [];

        public T Get<T>() where T : FixtureStatePart
        {
            var filteredStates = States.Where(s => s is T);
            if (!filteredStates.Any())
            {
                throw new InvalidOperationException();
            }
            return (T)filteredStates.First();
        }
    }
}
