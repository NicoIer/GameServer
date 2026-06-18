namespace zstd
{
    public class NativeTypeNameAttribute : System.Attribute
    {
        public string Name { get; }

        public NativeTypeNameAttribute(string name)
        {
            Name = name;
        }
    }
}