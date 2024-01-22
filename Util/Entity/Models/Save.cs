using System.ComponentModel.DataAnnotations;

namespace Util.Entity.Models;


public class Save
{
    [Key]
    public required ulong Id { get; set; }
    
    public required byte GameType { get; set; }
    
    public required byte[] Body { get; set; }
    
    public required DateTime CreatedAt { get; set; }
}