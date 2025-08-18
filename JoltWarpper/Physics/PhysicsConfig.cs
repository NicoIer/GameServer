namespace GameCore.Physics
{
    public class PhysicsConfig
    {
        public bool lockStep = false;
        public ushort lockStepPort = 24418;
        public static readonly PhysicsConfig Default = new PhysicsConfig();
    }
}