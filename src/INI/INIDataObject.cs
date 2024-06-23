using COILib.Extensions;
using COILib.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace COILib.INI;

public class IniData {
	public string Tag { get; set; }
	public string Comment { get; set; }

	protected IniData(string tag, string comment = null) {
		Tag = tag;
		Comment = comment;
	}

	public override string ToString() {
		return Tag;
	}
}

public class IniSectionData : IniData {

	public IniSectionData(IniLoader loader, string tag, string comment = null) : base(tag, comment) {
		loader.CreateSection(this);
	}
}

public class IniKeyEntry : IniData {
	public string StrValue { get; protected set; }

	public IniKeyEntry(string tag, string strValue, string comment = null) : base(tag, comment) {
		StrValue = strValue;
	}

	public override string ToString() {
		return Tag + ": " + StrValue;
	}
}

public abstract class AIniKeyData : IniKeyEntry {
	public IniSectionData IniSectionData { get; private set; }
	public string Key { get; private set; }

	protected AIniKeyData(IniSectionData iniSectionData, string key, string comment = null, string tag = "add") : base(tag, null, comment) {
		IniSectionData = iniSectionData;
		Key = key;
	}

	public abstract T GetValue<T>();
}

public class IniKeyData<T> : AIniKeyData where T : IComparable, IConvertible {
	private T[] AcceptedValues { get; set; }
	public T DefaultValue { get; private set; }
	private T Value { get; set; }
	private readonly IniLoader m_loader;

	public IniKeyData(IniLoader loader, IniSectionData iniSectionData, string key, T defaultValue, string comment = null, string tag = "add") : this(loader, iniSectionData, key, null, defaultValue, comment, tag) {
	}

	public IniKeyData(IniLoader loader, IniSectionData iniSectionData, string key, T[] acceptedValues, T defaultValue, string comment = null, string tag = "add") : base(iniSectionData, key, comment, tag) {
		m_loader = loader;
		DefaultValue = defaultValue;
		AcceptedValues = acceptedValues;
		SetStrValue(loader.GetValueOrCreate(this));
	}

	public void SetStrValue(string str, bool save = false) {
		StrValue = str;
		Value = convert(StrValue);
		m_loader.UpdateValue(this);
		if (save) {
			m_loader.Save();
		}
	}

	public override TV GetValue<TV>() => (TV)(object)Value;

	public T GetValue() => Value;

	private T convert(string value) {
		try {
			if (typeof(T) == typeof(string)) {
				return (T)(object)value;
			}

			if (typeof(T) == typeof(int) && int.TryParse(value, out int intValue)) {
				return (T)(object)intValue.Between(Convert.ToInt32(AcceptedValues[0]), Convert.ToInt32(AcceptedValues[1]));
			}

			if (typeof(T) == typeof(float) && float.TryParse(value, out float floatValue)) {
				return (T)(object)floatValue.Between(Convert.ToSingle(AcceptedValues[0]), Convert.ToSingle(AcceptedValues[1]));
			}

			if (typeof(T) == typeof(bool) && bool.TryParse(value, out bool boolValue) && (AcceptedValues == null || AcceptedValues.Contains((T)(object)boolValue))) {
				return (T)(object)boolValue;
			}

			if (typeof(T) == typeof(KeyCode) && Enum.TryParse(value, out KeyCode keyCode) && (AcceptedValues == null || AcceptedValues.Contains((T)(object)keyCode))) {
				return (T)(object)keyCode;
			}
		}
		catch (Exception) {
			ExtLog.Warning($"Error while reading and converting the config value of {Key}");
		}

		return DefaultValue;
	}

	public string[] ConvertToStringArray(bool clean = true, char separator = '|') {
		string[] acceptedValues = AcceptedValues as string[];

		try {
			string[] array = Value.ToString().Split(separator);
			List<string> stringList = [];
			foreach (string item in array) {
				string str = clean ? item.CleanString() : item;
				if (str.Trim().Length > 0 && (acceptedValues == null || acceptedValues.Length == 0 || acceptedValues.Contains(str))) {
					stringList.Add(str);
				}
			}
			return stringList.ToArray();
		}
		catch (Exception) {
			ExtLog.Warning($"Error while reading and converting the config value of {Key}");
		}

		return acceptedValues;
	}

	public override string ToString() {
		return Value.ToString();
	}
}