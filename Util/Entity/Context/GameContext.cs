using Microsoft.EntityFrameworkCore;
using Util.Entity.Models;

namespace Util.Entity.Context;

public class GameContext : DbContext
{
    public required DbSet<Save> Saves { get; set; }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(
            "server=20.214.207.225;database=game;user=jipark;password=Aa490661!",
            new MySqlServerVersion("8.3.0"));
    }
}