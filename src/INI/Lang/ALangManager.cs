using COILib.Extensions;
using COILib.General;
using Mafi;
using Mafi.Collections;
using Mafi.Localization;
using System;

namespace COILib.INI.Lang;

public enum StringType {
    Normal,
    Capitalize,
    CapitalizeAll,
    UpperCase,
    LowerCase,
}

public abstract class ALangManager<TL, T>(IniLoader loader) : InstanceObject<TL> where T : Enum where TL : ALangManager<TL, T>, new() {
    private string m_currentLang;
    protected IniSectionData LangSection;
    private Dict<T, IniKeyData<string>> m_keyValues;
    protected readonly IniLoader m_loader = loader;

    protected ALangManager(string path) : this(new IniLoader(path)) {
    }

    protected abstract Dict<T, IniKeyData<string>> GetKeyValues();

    protected override void OnInit() {
        m_currentLang = LocalizationManager.CurrentLangInfo.LanguageTitle;
        LangSection = new IniSectionData(m_loader, m_currentLang, $"Keywords for translating to the {m_currentLang} language");
        m_keyValues = GetKeyValues();
        m_loader.Save();
    }

    public string Get(T key, StringType types, params string[] toReplace) {
        string result = m_keyValues.TryGetValue(key, out IniKeyData<string> data) ? data.GetValue() : key.ToString();
        if (toReplace?.Length > 0) {
            for (int i = 0; i <= toReplace.Length; i++) {
                result = result.Replace("{" + i + "}", toReplace[i]);
            }
        }

        return types switch {
            StringType.UpperCase => result.ToUpper(),
            StringType.LowerCase => result.ToLower(),
            StringType.Capitalize => result.CapitalizeFirstChar(),
            StringType.CapitalizeAll => result.CapitalizeAllFirstChar(),
            StringType.Normal => result,
            _ => result
        };
    }

    public string Get(T key, params string[] toReplace) => Get(key, StringType.Normal, toReplace);

    public LocStrFormatted GetAsLoc(T key, params string[] toReplace) => GetAsLoc(key, StringType.Normal, toReplace);

    public LocStrFormatted GetAsLoc(T key, StringType types, params string[] toReplace) => Get(key, types, toReplace).AsLoc();
}