using System.Text.Json;

static class LanguageManager
{
    private static Dictionary<string, string>? texts;

    public static void Update(){LoadLanguage();}

    private static void LoadLanguage()
    {
        try
        {
            string language = Preferences.Get("language", "en");
            string path = Path.Combine(AppContext.BaseDirectory, "Resources", "language", $"strings_{language}.json");
            if (!File.Exists(path))
            {
                texts = new Dictionary<string, string>();
                return;
            }
            string json = File.ReadAllText(path);
            texts = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }
        catch (Exception)
        {
            texts = new Dictionary<string, string>();
            // Optional: Logging ex.Message
        }
    }

    public static string GetText(string key)
    {
        if (texts == null || texts.Count == 0)
            Update();
        return texts != null && texts.TryGetValue(key, out var value) ? value : key;
    }
}