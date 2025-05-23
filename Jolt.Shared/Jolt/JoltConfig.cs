namespace GameCore.Jolt
{
    public class JoltConfig
    {
        public bool lockStep = false;
        public ushort lockStepPort = 24418;
        public static readonly JoltConfig Default = new JoltConfig();
    }
}