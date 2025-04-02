using System.IO;
using System.Text.Json;

namespace TitanStudioWpf.Core.Helpers.Deserializers;

public class JsonHelper
{
    public static List<T> ReadJsonData<T>(string jsonFilePath, string key, Func<JsonElement, List<T>> transform) where T : new()
    {
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(jsonFilePath));

        if (data != null && data.ContainsKey(key))
        {
            return transform(data[key]);
        }
        else
        {
            return new List<T>();
        }
    }
}
