using System.Text.Json.Nodes;

namespace OSManager.Daemon.Extensions;

public static class JsonObjectExtensions
{
    public static string[] GetKeys(this JsonObject jsonObject)
    {
        string[] keys = new string[jsonObject.Count];
        int i = 0;
        foreach (KeyValuePair<string, JsonNode?> keyValuePair in jsonObject)
        {
            keys[i] = keyValuePair.Key;
            i++;
        }

        return keys;
    }
}