using System;
using System.Collections.Generic;
using System.IO;

namespace COILib.INI;

public class IniLoader(string path, bool readOnly = false) {
	private string Path { get; } = path;
	private readonly Dictionary<string, Dictionary<string, IniKeyEntry>> m_data = loadIni(path);

	public Dictionary<string, Dictionary<string, IniKeyEntry>> GetData() => m_data;

	private static Dictionary<string, Dictionary<string, IniKeyEntry>> loadIni(string path) {
		var data = new Dictionary<string, Dictionary<string, IniKeyEntry>>(StringComparer.OrdinalIgnoreCase);
		if (!File.Exists(path)) {
			return data;
		}

		string[] lines = File.ReadAllLines(path);
		Dictionary<string, IniKeyEntry> currentSection = null;
		foreach (string line in lines) {
			string trimmedLine = line.Trim();
			if (string.IsNullOrEmpty(trimmedLine)) {
				continue;
			}

			if (trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#")) {
				continue;
			}

			if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]")) {
				string currentSectionName = trimmedLine.Substring(1, trimmedLine.Length - 2).Trim();
				if (data.ContainsKey(currentSectionName)) {
					continue;
				}

				currentSection = new Dictionary<string, IniKeyEntry>(StringComparer.OrdinalIgnoreCase);
				data[currentSectionName] = currentSection;
			}
			else if (currentSection != null) {
				string[] keyValue = trimmedLine.Split(['='], 2);
				if (keyValue.Length != 2) {
					continue;
				}

				string key = keyValue[0].Trim();
				string value = keyValue[1].Trim();
				currentSection[key] = new IniKeyEntry(key, value);
			}
		}
		return data;
	}

	public void CreateSection(IniSectionData x) {
		if (!m_data.ContainsKey(x.Tag)) {
			m_data[x.Tag] = new Dictionary<string, IniKeyEntry>(StringComparer.OrdinalIgnoreCase);
		}

		if (x.Comment != null) {
			m_data[x.Tag]["_section_comment"] = new IniKeyEntry("_section_comment", null, x.Comment);
		}
	}

	public string GetValueOrCreate<T>(IniKeyData<T> x) where T : IComparable, IConvertible {
		if (!m_data.TryGetValue(x.IniSectionData.Tag, out var sectionData)) {
			sectionData = new Dictionary<string, IniKeyEntry>(StringComparer.OrdinalIgnoreCase);
			m_data[x.IniSectionData.Tag] = sectionData;
		}

		if (!sectionData.TryGetValue(x.Key, out var entry)) {
			entry = new IniKeyEntry(x.Key, x.DefaultValue.ToString(), x.Comment);
			sectionData[x.Key] = entry;
		}

		if (x.Comment != null) {
			entry.Comment = x.Comment;
		}

		return entry.StrValue;
	}

	public void UpdateValue<T>(IniKeyData<T> x) where T : IComparable, IConvertible {
		if (!m_data.TryGetValue(x.IniSectionData.Tag, out var sectionData)) {
			sectionData = new Dictionary<string, IniKeyEntry>(StringComparer.OrdinalIgnoreCase);
			m_data[x.IniSectionData.Tag] = sectionData;
		}
		sectionData[x.Key] = x;
	}

	public void Save() {
		if (readOnly) {
			return;
		}

		var lines = new List<string>();
		foreach (var section in m_data) {
			if (section.Value.TryGetValue("_section_comment", out IniKeyEntry value)) {
				string comment = value.Comment;
				if (!string.IsNullOrEmpty(comment)) {
					addCommentLines(lines, comment);
				}
			}
			lines.Add($"[{section.Key}]");
			foreach (var keyValue in section.Value) {
				if (keyValue.Key == "_section_comment") {
					continue;
				}

				if (!string.IsNullOrEmpty(keyValue.Value.Comment)) {
					addCommentLines(lines, keyValue.Value.Comment);
				}

				lines.Add($"{keyValue.Key}={keyValue.Value.StrValue}");
			}
			lines.Add(string.Empty);
		}
		File.WriteAllLines(Path, lines);
	}

	private static void addCommentLines(List<string> lines, string comment) {
		string[] commentLines = comment.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		foreach (string line in commentLines) {
			lines.Add($"# {line}");
		}
	}
}