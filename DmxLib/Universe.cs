using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DmxLib.Rendering;

namespace DmxLib
{
    public class Universe
    {
        public int Id { get; init; }

        public IReadOnlyList<Fixture> Fixtures { get; init; }

        public RenderFrame CreateState()
        {
            return new RenderFrame(Fixtures.Select(f => (f, f.Definition.CreateState())).ToDictionary());
        }
    }
}
