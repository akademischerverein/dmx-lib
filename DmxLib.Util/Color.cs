using System;

namespace DmxLib.Util
{
    public class Color
    {
        private Color(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }

        public double R { get; private set; }
        public double G { get; private set; }
        public double B { get; private set; }
        public double H
        {
            get
            {
                double h = 0;
                double max = Math.Max(R, Math.Max(G, B));
                double min = Math.Min(R, Math.Min(G, B));

                if (max == min)
                {
                    h = 0;
                } else if (max == R)
                {
                    h = 60 * ((G - B) / (max - min));
                } else if (max == G)
                {
                    h = 60 * (2 + (B - R) / (max - min));
                } else if (max == B)
                {
                    h = 60 * (4 + (R - G) / (max - min));
                }

                if (h < 0)
                {
                    h += 360;
                } else if (h > 360)
                {
                    h -= 360;
                }
                return h;
            }
        }

        public double S
        {
            get
            {
                double max = Math.Max(R, Math.Max(G, B));
                double min = Math.Min(R, Math.Min(G, B));

                if (max == 0.0)
                {
                    return 0;
                }
                else
                {
                    return (max - min) / max;
                }
            }
        }

        public double V
        {
            get { return Math.Max(R, Math.Max(G, B)); }
        }

        public Color Copy()
        {
            return new Color(R, G, B);
        }

        public static Color FromRGB(double r, double g, double b)
        {
            if (r < 0.0 || r > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(r), "Red must be in the range of 0-1");
            }
            if (g < 0.0 || g > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(g), "Green must be in the range of 0-1");
            }
            if (b < 0.0 || b > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(b), "Blue must be in the range of 0-1");
            }

            return new Color(r, g, b);
        }

        public static Color FromHSV(double h, double s, double v)
        {
            if (h < 0.0 || h > 360.0)
            {
                throw new ArgumentOutOfRangeException(nameof(h), "Hue must be in the range of 0-360");
            }
            if (s < 0.0 || s > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(s), "Saturation must be in the range of 0-1");
            }
            if (v < 0.0 || v > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Value must be in the range of 0-1");
            }
            
            int hi = (int)(h / 60.0);
            double f = h / 60.0 - hi;

            double p = v * (1 - s);
            double q = v * (1 - s * f);
            double t = v * (1 - s * (1 - f));

            switch (hi)
            {
                case 0:
                case 6:
                    return new Color(v, t, p);
                case 1:
                    return new Color(q, v, p);
                case 2:
                    return new Color(p, v, t);
                case 3:
                    return new Color(p, q, v);
                case 4:
                    return new Color(t, p, v);
                case 5:
                    return new Color(v, p, q);
                default:
                    throw new ArgumentException();
            }
        }
    }
}