using Microsoft.EntityFrameworkCore;
using Util.Entity.Models;

namespace Util.Entity.Context;

public class GameContext : DbContext
{
    public required DbSet<Save> Saves { get; init; }
}