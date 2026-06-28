using System;
using System.Collections.Generic;
using System.Text;

namespace DmxLib.Rendering
{
    public class UniverseRenderer
    {
        public UniverseRenderer(FixtureRenderer fixRenderer)
        {
            renderer = fixRenderer;
        }

        private readonly FixtureRenderer renderer;

        public Memory<byte> Render(RenderFrame renderState)
        {
            Memory<byte> buffer = new byte[512];
            foreach(var (fix, state) in renderState)
            {
                renderer.Render(fix, state, buffer);
            }

            return buffer;
        }
    }
}
