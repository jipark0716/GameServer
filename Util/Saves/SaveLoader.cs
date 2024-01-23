using System.Text.Json;
using Util.Entity.Context;
using Util.Extensions;

namespace Util.Saves;

public class SaveRepository(GameContext context)
{
    public async Task Save<T>(T source, ulong id)
        where T : class
    {
        var save = await context.Saves.FindAsync(id);
        if (save is null) return;
        save.Body = source.ToJsonByte();
        await context.SaveChangesAsync();
    }
    
    public async Task<ulong> Save<T>(T source, byte gameType)
        where T : class
    {
        var result = await context.Saves.AddAsync(new()
        {
            GameType = gameType,
            Body = source.ToJsonByte()
        });
        
        return result.Entity.Id;
    }

    public async Task<T?> Load<T>(ulong id, byte gameType)
        where T : class
    {
        var save = await context.Saves.FindAsync(id);
        return save is null ? null : JsonSerializer.Deserialize<T>(save.Body);
    }
}