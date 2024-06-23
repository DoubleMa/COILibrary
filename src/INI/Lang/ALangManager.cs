using COILib.Extensions;
using COILib.General;
using Mafi;
using Mafi.Collections;
using Mafi.Localization;
using System;

namespace COILib.INI.Lang {

    public enum StringType {
        Normal,
        Capitalize,
        CapitalizeAll,
        UpperCase,
        LowerCase,
    }

    public abstract class ALangManager<L, T> : InstanceObject<L> where T : Enum where L : ALangManager<L, T>, new() {
        private string CurrentLang;
        protected IniSectionData LangSection;
        private Dict<T, IniKeyData<string>> keyValues;
        protected INILoader loader;

        public ALangManager(string path) : this(new INILoader(path)) {
        }

        public ALangManager(INILoader loader) {
            this.loader = loader;
        }

        protected abstract Dict<T, IniKeyData<string>> GetKeyValues();

        protected override void OnInit() {
            CurrentLang = LocalizationManager.CurrentLangInfo.LanguageTitle;
            LangSection = new IniSectionData(loader, CurrentLang, $"Keywords for translating to the {CurrentLang} language");
            keyValues = GetKeyValues();
            loader.Save();
        }

        public string Get(T key, StringType types, params string[] toReplace) {
            string result = keyValues.TryGetValue(key, out IniKeyData<string> data) ? data.GetValue() : key.ToString();
            if (toReplace != null && toReplace.Length > 0) for (int i = 0; i <= toReplace.Length; i++) result = result.Replace("{" + i + "}", toReplace[i]);
            switch (types) {
                case StringType.UpperCase: return result.ToUpper();
                case StringType.LowerCase: return result.ToLower();
                case StringType.Capitalize: return result.CapitalizeFirstChar();
                case StringType.CapitalizeAll: return result.CapitalizeAllFirstChar();
                case StringType.Normal:
                default:
                    return result;
            }
        }

        public string Get(T key, params string[] toReplace) => Get(key, StringType.Normal, toReplace);

        public LocStrFormatted GetAsLoc(T key, params string[] toReplace) => GetAsLoc(key, StringType.Normal, toReplace);

        public LocStrFormatted GetAsLoc(T key, StringType types, params string[] toReplace) => Get(key, types, toReplace).AsLoc();
    }
}