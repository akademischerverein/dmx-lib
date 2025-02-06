namespace DmxLib.Util
{
    public class Vector2f
    {
        public Vector2f(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public int X { get; private set; }
        public int Y { get; private set; }
    }
}