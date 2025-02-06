namespace DmxLib
{
    public struct Cue
    {
        public IDevice Device;
        public DeviceProperty Property;
        public object Value;
        public uint Delay; // in ms
        public uint Fade; // in ms
    }
}