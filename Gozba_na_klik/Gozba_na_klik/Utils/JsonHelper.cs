using System.Text.Json;

namespace Gozba_na_klik.Utils
{
    public static class JsonHelper
    {
        public static List<string> DeserializeStringList(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch (JsonException)
            {
                return new List<string>();
            }
        }
    }
}