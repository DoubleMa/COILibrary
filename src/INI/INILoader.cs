using System;
using System.Collections.Generic;
using System.IO;

namespace COILib.INI {

    public class INILoader {
        public string path { get; }
        private readonly bool readOnly;
        protected readonly Dictionary<string, Dictionary<string, IniKeyEntry>> data;

        public INILoader(string path, bool readOnly = false) {
            this.path = path;
            this.readOnly = readOnly;
            data = LoadIni(path);
        }

        public Dictionary<string, Dictionary<string, IniKeyEntry>> getData() => data;

        protected Dictionary<string, Dictionary<string, IniKeyEntry>> LoadIni(string path) {
            var data = new Dictionary<string, Dictionary<string, IniKeyEntry>>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(path)) return data;
            var lines = File.ReadAllLines(path);
            Dictionary<string, IniKeyEntry> currentSection = null;
            string currentSectionName;
            foreach (var line in lines) {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;
                if (trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#")) continue;
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]")) {
                    currentSectionName = trimmedLine.Substring(1, trimmedLine.Length - 2).Trim();
                    if (!data.ContainsKey(currentSectionName)) {
                        currentSection = new Dictionary<string, IniKeyEntry>(StringComparer.OrdinalIgnoreCase);
                        data[currentSectionName] = currentSection;
                    }
                }
                else if (currentSection != null) {
                    var keyValue = trimmedLine.Split(new[] { '=' }, 2);
                    if (keyValue.Length == 2) {
                        var key = keyValue[0].Trim();
                        var value = keyValue[1].Trim();
                        currentSection[key] = new IniKeyEntry(key, value);
                    }
                }
            }
            return data;
        }

        public void CreateSection(IniSectionData x) {
            if (!data.ContainsKey(x.Tag)) data[x.Tag] = new Dictionary<string, IniKeyEntry>(StringComparer.OrdinalIgnoreCase);
            if (x.Comment != null) data[x.Tag]["_section_comment"] = new IniKeyEntry("_section_comment", null, x.Comment);
        }

        public string GetValueOrCreate<T>(IniKeyData<T> x) where T : IComparable, IConvertible {
            if (!data.TryGetValue(x.IniSectionData.Tag, out var sectionData)) {
                sectionData = new Dictionary<string, IniKeyEntry>(StringComparer.OrdinalIgnoreCase);
                data[x.IniSectionData.Tag] = sectionData;
            }
            if (!sectionData.TryGetValue(x.Key, out var entry)) {
                entry = new IniKeyEntry(x.Key, x.DefaultValue.ToString(), x.Comment);
                sectionData[x.Key] = entry;
            }
            if (x.Comment != null) entry.Comment = x.Comment;
            return entry.StrValue;
        }

        public void UpdateValue<T>(IniKeyData<T> x) where T : IComparable, IConvertible {
            if (!data.TryGetValue(x.IniSectionData.Tag, out var sectionData)) {
                sectionData = new Dictionary<string, IniKeyEntry>(StringComparer.OrdinalIgnoreCase);
                data[x.IniSectionData.Tag] = sectionData;
            }
            sectionData[x.Key] = x;
        }

        public void Save() {
            if (readOnly) return;
            var lines = new List<string>();
            foreach (var section in data) {
                if (section.Value.ContainsKey("_section_comment")) {
                    var comment = section.Value["_section_comment"].Comment;
                    if (!string.IsNullOrEmpty(comment)) AddCommentLines(lines, comment);
                }
                lines.Add($"[{section.Key}]");
                foreach (var keyValue in section.Value) {
                    if (keyValue.Key == "_section_comment") continue;
                    if (!string.IsNullOrEmpty(keyValue.Value.Comment)) AddCommentLines(lines, keyValue.Value.Comment);
                    lines.Add($"{keyValue.Key}={keyValue.Value.StrValue}");
                }
                lines.Add(string.Empty);
            }
            File.WriteAllLines(path, lines);
        }

        private void AddCommentLines(List<string> lines, string comment) {
            var commentLines = comment.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in commentLines) lines.Add($"# {line}");
        }
    }
}