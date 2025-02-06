namespace DmxLib
{
    public interface ISink
    {
        void Update(Universe universe, byte[] values);
    }
}