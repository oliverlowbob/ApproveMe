namespace ApproveMe.FileParsers;

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

public class JsonFileParser : IFileParser
{
    public IEnumerable<IDictionary<string, object>> Parse(Stream fileStream, IEnumerable<int> selectedRows)
    {
        var selectedRowSet = new HashSet<int>(selectedRows);
        using var reader = new StreamReader(fileStream);
        var json = reader.ReadToEnd();

        // Expecting a JSON array at the root
        var jArray = JArray.Parse(json);
        for (var i = 0; i < jArray.Count; i++)
        {
            if (selectedRowSet.Contains(i))
            {
                var jToken = jArray[i];
                // Flatten the JSON object if needed
                IDictionary<string, object> record = FlattenJToken(jToken);
                yield return record;
            }
        }
    }

    private IDictionary<string, object> FlattenJToken(JToken token, string prefix = "")
    {
        var result = new Dictionary<string, object>();

        if (token is JObject jObject)
        {
            foreach (var prop in jObject.Properties())
            {
                var propName = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}_{prop.Name}";
                foreach (var kvp in FlattenJToken(prop.Value, propName))
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
        }
        else if (token is JArray jArray)
        {
            // For arrays, you can decide to join the values as a comma-separated string
            result[prefix] = string.Join(", ", jArray);
        }
        else
        {
            result[prefix] = token.ToString();
        }
        return result;
    }
}
