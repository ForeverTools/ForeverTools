namespace ForeverTools.Translate;

/// <summary>
/// Represents a language with its code and name.
/// </summary>
public class Language
{
    /// <summary>
    /// ISO 639-1 language code (e.g., "en", "es", "fr").
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// English name of the language.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Native name of the language.
    /// </summary>
    public string NativeName { get; }

    /// <summary>
    /// Creates a new Language instance.
    /// </summary>
    public Language(string code, string name, string nativeName)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        NativeName = nativeName ?? throw new ArgumentNullException(nameof(nativeName));
    }

    public override string ToString() => $"{Name} ({Code})";

    public override bool Equals(object? obj)
    {
        return obj is Language other && Code.Equals(other.Code, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => Code.ToLowerInvariant().GetHashCode();

    /// <summary>
    /// Implicitly converts a language code string to a Language object.
    /// </summary>
    public static implicit operator Language(string code) => FromCode(code);

    /// <summary>
    /// Gets a Language from its code, or creates a generic one if not found.
    /// </summary>
    public static Language FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Language code is required.", nameof(code));

        var normalized = code.ToLowerInvariant().Trim();
        return Languages.All.TryGetValue(normalized, out var lang) ? lang : new Language(normalized, normalized, normalized);
    }
}

/// <summary>
/// Common language codes for convenience.
/// </summary>
public static class Languages
{
    // Major World Languages
    public static readonly Language English = new("en", "English", "English");
    public static readonly Language Spanish = new("es", "Spanish", "Español");
    public static readonly Language French = new("fr", "French", "Français");
    public static readonly Language German = new("de", "German", "Deutsch");
    public static readonly Language Italian = new("it", "Italian", "Italiano");
    public static readonly Language Portuguese = new("pt", "Portuguese", "Português");
    public static readonly Language Russian = new("ru", "Russian", "Русский");
    public static readonly Language Japanese = new("ja", "Japanese", "日本語");
    public static readonly Language Korean = new("ko", "Korean", "한국어");
    public static readonly Language Chinese = new("zh", "Chinese", "中文");
    public static readonly Language ChineseSimplified = new("zh-CN", "Chinese (Simplified)", "简体中文");
    public static readonly Language ChineseTraditional = new("zh-TW", "Chinese (Traditional)", "繁體中文");
    public static readonly Language Arabic = new("ar", "Arabic", "العربية");
    public static readonly Language Hindi = new("hi", "Hindi", "हिन्दी");
    public static readonly Language Bengali = new("bn", "Bengali", "বাংলা");
    public static readonly Language Vietnamese = new("vi", "Vietnamese", "Tiếng Việt");
    public static readonly Language Thai = new("th", "Thai", "ไทย");
    public static readonly Language Indonesian = new("id", "Indonesian", "Bahasa Indonesia");
    public static readonly Language Malay = new("ms", "Malay", "Bahasa Melayu");
    public static readonly Language Turkish = new("tr", "Turkish", "Türkçe");

    // European Languages
    public static readonly Language Dutch = new("nl", "Dutch", "Nederlands");
    public static readonly Language Polish = new("pl", "Polish", "Polski");
    public static readonly Language Swedish = new("sv", "Swedish", "Svenska");
    public static readonly Language Norwegian = new("no", "Norwegian", "Norsk");
    public static readonly Language Danish = new("da", "Danish", "Dansk");
    public static readonly Language Finnish = new("fi", "Finnish", "Suomi");
    public static readonly Language Greek = new("el", "Greek", "Ελληνικά");
    public static readonly Language Czech = new("cs", "Czech", "Čeština");
    public static readonly Language Romanian = new("ro", "Romanian", "Română");
    public static readonly Language Hungarian = new("hu", "Hungarian", "Magyar");
    public static readonly Language Ukrainian = new("uk", "Ukrainian", "Українська");
    public static readonly Language Bulgarian = new("bg", "Bulgarian", "Български");
    public static readonly Language Croatian = new("hr", "Croatian", "Hrvatski");
    public static readonly Language Slovak = new("sk", "Slovak", "Slovenčina");
    public static readonly Language Slovenian = new("sl", "Slovenian", "Slovenščina");
    public static readonly Language Serbian = new("sr", "Serbian", "Српски");
    public static readonly Language Catalan = new("ca", "Catalan", "Català");

    // Middle Eastern / South Asian
    public static readonly Language Hebrew = new("he", "Hebrew", "עברית");
    public static readonly Language Persian = new("fa", "Persian", "فارسی");
    public static readonly Language Urdu = new("ur", "Urdu", "اردو");
    public static readonly Language Tamil = new("ta", "Tamil", "தமிழ்");
    public static readonly Language Telugu = new("te", "Telugu", "తెలుగు");
    public static readonly Language Marathi = new("mr", "Marathi", "मराठी");
    public static readonly Language Gujarati = new("gu", "Gujarati", "ગુજરાતી");
    public static readonly Language Kannada = new("kn", "Kannada", "ಕನ್ನಡ");
    public static readonly Language Malayalam = new("ml", "Malayalam", "മലയാളം");
    public static readonly Language Punjabi = new("pa", "Punjabi", "ਪੰਜਾਬੀ");

    // African Languages
    public static readonly Language Swahili = new("sw", "Swahili", "Kiswahili");
    public static readonly Language Afrikaans = new("af", "Afrikaans", "Afrikaans");
    public static readonly Language Amharic = new("am", "Amharic", "አማርኛ");

    // Other
    public static readonly Language Filipino = new("tl", "Filipino", "Filipino");
    public static readonly Language Icelandic = new("is", "Icelandic", "Íslenska");
    public static readonly Language Irish = new("ga", "Irish", "Gaeilge");
    public static readonly Language Welsh = new("cy", "Welsh", "Cymraeg");
    public static readonly Language Latvian = new("lv", "Latvian", "Latviešu");
    public static readonly Language Lithuanian = new("lt", "Lithuanian", "Lietuvių");
    public static readonly Language Estonian = new("et", "Estonian", "Eesti");

    /// <summary>
    /// Dictionary of all supported languages by code.
    /// </summary>
    public static IReadOnlyDictionary<string, Language> All { get; } = new Dictionary<string, Language>(StringComparer.OrdinalIgnoreCase)
    {
        ["en"] = English,
        ["es"] = Spanish,
        ["fr"] = French,
        ["de"] = German,
        ["it"] = Italian,
        ["pt"] = Portuguese,
        ["ru"] = Russian,
        ["ja"] = Japanese,
        ["ko"] = Korean,
        ["zh"] = Chinese,
        ["zh-cn"] = ChineseSimplified,
        ["zh-tw"] = ChineseTraditional,
        ["ar"] = Arabic,
        ["hi"] = Hindi,
        ["bn"] = Bengali,
        ["vi"] = Vietnamese,
        ["th"] = Thai,
        ["id"] = Indonesian,
        ["ms"] = Malay,
        ["tr"] = Turkish,
        ["nl"] = Dutch,
        ["pl"] = Polish,
        ["sv"] = Swedish,
        ["no"] = Norwegian,
        ["da"] = Danish,
        ["fi"] = Finnish,
        ["el"] = Greek,
        ["cs"] = Czech,
        ["ro"] = Romanian,
        ["hu"] = Hungarian,
        ["uk"] = Ukrainian,
        ["bg"] = Bulgarian,
        ["hr"] = Croatian,
        ["sk"] = Slovak,
        ["sl"] = Slovenian,
        ["sr"] = Serbian,
        ["ca"] = Catalan,
        ["he"] = Hebrew,
        ["fa"] = Persian,
        ["ur"] = Urdu,
        ["ta"] = Tamil,
        ["te"] = Telugu,
        ["mr"] = Marathi,
        ["gu"] = Gujarati,
        ["kn"] = Kannada,
        ["ml"] = Malayalam,
        ["pa"] = Punjabi,
        ["sw"] = Swahili,
        ["af"] = Afrikaans,
        ["am"] = Amharic,
        ["tl"] = Filipino,
        ["is"] = Icelandic,
        ["ga"] = Irish,
        ["cy"] = Welsh,
        ["lv"] = Latvian,
        ["lt"] = Lithuanian,
        ["et"] = Estonian
    };
}
