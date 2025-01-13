using Microsoft.EntityFrameworkCore;

namespace FishGame;

[PrimaryKey(nameof(id))]
public sealed class UintConfig
{
    public string id { get; set; }
    public uint value { get; set; }
}