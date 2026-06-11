using System.Collections;
using MemoryPack;

namespace TestProject;

public class BitArrayTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        // int[] m_array
        // int m_length
        // int _version
        
        // 1 byte = 8 bits
        // 1 int = 4 bytes = 32 bits
        
        // 0 -> 9 bytes
        // 1 -> 13 bytes
        // 33 -> 17 bytes
        // 65 -> 61 bytes
        
        
        BitArray bitArray = new BitArray(65);
        var bytes = MemoryPackSerializer.Serialize(bitArray);
        TestContext.Out.WriteLine($"Serialized BitArray to {bytes.Length} bytes.");
        
    }
}