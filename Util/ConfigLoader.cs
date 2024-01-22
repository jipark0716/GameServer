using System.Text.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Util;

public class ConfigLoader
{
    private string GetFilePath(string ver)
        => $"config_{ver}.json";
    
    private string GetFromKeyVault(string resource, string key, string ver)
    {
        Console.WriteLine("load from ket vault");
        
        var client = new SecretClient(
            new Uri($"https://{resource}.vault.azure.net"),
            new DefaultAzureCredential());

        var result = client.GetSecret(key, ver).Value.Value;
        File.WriteAllTextAsync(GetFilePath(ver), result);
        return result;
    }

    private string? GetFromCache(string ver)
    {
        var configPath = GetFilePath(ver);
        
        if (File.Exists(configPath) is false)
            return null;
        
        Console.WriteLine("load from file");

        using var sr = new StreamReader(configPath);
        return sr.ReadToEnd();
    }

    public T Get<T>(string resource, string key, string ver)
        where T : class
    {
        var body = GetFromCache(ver) ?? GetFromKeyVault(resource, key, ver);
        
        Console.Write($"config load {body}");
        
        return JsonSerializer.Deserialize<T>(body) ?? throw new Exception("config load fail");
    }
}