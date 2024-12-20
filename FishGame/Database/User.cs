using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FishGame
{
    [PrimaryKey(nameof(id))]
    [Index(nameof(nickname), IsUnique = true)]
    [Index(nameof(uid), IsUnique = true)]
    public sealed class User
    {
        public int id;
        public uint uid;
        public required string nickname;

        public bool online { get; set; }
    }
}