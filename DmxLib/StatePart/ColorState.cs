using System;
using System.Collections.Generic;
using System.Text;
using DmxLib.Util;

namespace DmxLib.StatePart
{
    public class ColorState : FixtureStatePart
    {
        public Color Color { get; set; } = Color.FromRGB(0, 0, 0);
    }
}
