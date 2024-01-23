using System.ComponentModel.DataAnnotations;

namespace Util.Entity.Models;


public class Save
{
    [Key]
    public ulong Id { get; init; }
    
    public required byte GameType { get; init; }
    
    public required byte[] Body { get; set; }
    
    public DateTime CreatedAt { get; init; }
}