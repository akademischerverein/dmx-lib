using DmxLib;
using DmxLib.Rendering;

namespace DmxLib.Test
{
    internal static class RendererTestFactory
    {
        public static RenderingResolver CreateResolver()
        {
            return new RenderingResolver
            {
                Renderer =
                [
                    new ColorRenderer(),
                    new DimmerRenderer(),
                    new RelayRenderer(),
                ],
            };
        }

        public static FixtureRenderer CreateFixtureRenderer()
        {
            return new FixtureRenderer
            {
                Resolver = CreateResolver(),
            };
        }

        public static UniverseRenderer CreateUniverseRenderer()
        {
            return new UniverseRenderer(CreateFixtureRenderer());
        }
    }
}
