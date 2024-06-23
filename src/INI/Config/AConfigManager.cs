using COILib.General;
using Mafi.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COILib.INI.Config;

public abstract class AConfigManager<TL, T> : InstanceObject<TL> where T : Enum where TL : AConfigManager<TL, T>, new() {
	private Dict<T, AIniKeyData> m_keyValues;
	private readonly IniLoader m_loader;

	private readonly KeyCode[][] m_acceptedKeyCodes = [
		[KeyCode.None, KeyCode.Backspace, KeyCode.Delete, KeyCode.Tab, KeyCode.Return, KeyCode.Pause, KeyCode.Escape, KeyCode.Space],
		[KeyCode.Keypad0, KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6, KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9, KeyCode.KeypadPeriod, KeyCode.KeypadDivide, KeyCode.KeypadMultiply, KeyCode.KeypadMinus, KeyCode.KeypadPlus, KeyCode.KeypadEnter, KeyCode.KeypadEquals],
		[KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.Insert, KeyCode.Home, KeyCode.End, KeyCode.PageUp, KeyCode.PageDown],
		[KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5, KeyCode.F6, KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12, KeyCode.F13, KeyCode.F14, KeyCode.F15],
		[KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9],
		[KeyCode.Exclaim, KeyCode.DoubleQuote, KeyCode.Hash, KeyCode.Dollar, KeyCode.Percent, KeyCode.Ampersand, KeyCode.Quote, KeyCode.LeftParen, KeyCode.RightParen, KeyCode.Asterisk, KeyCode.Plus, KeyCode.Comma, KeyCode.Minus, KeyCode.Period, KeyCode.Slash, KeyCode.Colon, KeyCode.Semicolon, KeyCode.Less, KeyCode.Equals, KeyCode.Greater, KeyCode.Question, KeyCode.At],
		[KeyCode.LeftBracket, KeyCode.Backslash, KeyCode.RightBracket, KeyCode.Caret, KeyCode.Underscore, KeyCode.BackQuote],
		[KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z],
		[KeyCode.LeftCurlyBracket, KeyCode.RightCurlyBracket, KeyCode.Tilde, KeyCode.Numlock, KeyCode.CapsLock, KeyCode.ScrollLock],
		[KeyCode.RightShift, KeyCode.LeftShift, KeyCode.RightControl, KeyCode.LeftControl, KeyCode.RightAlt, KeyCode.LeftAlt, KeyCode.LeftMeta, KeyCode.LeftCommand, KeyCode.LeftApple, KeyCode.LeftWindows, KeyCode.RightMeta, KeyCode.RightCommand, KeyCode.RightApple, KeyCode.RightWindows],
		[KeyCode.AltGr, KeyCode.Help, KeyCode.Print, KeyCode.Break, KeyCode.Menu],
		[KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3, KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6]
	];

	public AConfigManager(string path) : this(new IniLoader(path)) {
	}

	public AConfigManager(IniLoader loader) {
		this.m_loader = loader;
	}

	protected abstract Dict<T, AIniKeyData> GetKeyValues();

	protected override void OnInit() {
		m_keyValues = GetKeyValues();
		m_loader.Save();
	}

	public TV Get<TV>(T key) {
		return m_keyValues.TryGetValue(key, out AIniKeyData data) ? data.GetValue<TV>() : throw new Exception($"Config Not found for {key}");
	}

	public string[] GetStringArray(T key) {
		try {
			if (GetKeyValues().TryGetValue(key, out AIniKeyData data)) {
				return ((IniKeyData<string>)data).ConvertToStringArray(false);
			}
		} catch { }

		return [];
	}

	protected string GenerateAcceptedKeyCodesComment(IEnumerable<KeyCode[]> acceptedKeyCodes) {
		var sb = new StringBuilder();
		sb.AppendLine("Set the keycode for each key.");
		sb.AppendLine("Accepted values (it's case-sensitive so make sure to copy paste the key name from this list, otherwise the default value will be selected):");
		sb.AppendLine();
		foreach (var keyGroup in acceptedKeyCodes) {
			sb.AppendLine("\t" + string.Join(", ", keyGroup.Select(k => k.ToString())));
		}

		return sb.ToString();
	}

	protected KeyCode[] FAcceptedKeyCodes() => m_acceptedKeyCodes.SelectMany(x => x).ToArray();
}