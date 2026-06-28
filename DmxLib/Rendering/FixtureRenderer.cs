using System;
using System.Collections.Generic;
using System.Text;

namespace DmxLib.Rendering
{
    public class FixtureRenderer
    {
        public required RenderingResolver Resolver { get; init; }

        public void Render(Fixture fixture, FixtureState state, Memory<byte> buffer)
        {
            var ctx = new RenderContext
            {
                Definition = fixture.Definition,
                State = state,
                Buffer = buffer,
                StartAddress = fixture.Address
            };

            foreach(var capability in fixture.Definition.Capabilities)
            {
                Resolver.Resolve(capability.GetType()).Render(ctx, capability);
            }
        }
    }
}
