using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FishGame;

[PrimaryKey(nameof(id))]
public sealed class LongConfig
{
    public required string id;
    public long value { get; set; }
}