using COILib.Extensions;
using COILib.General;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace COILib.Patcher;

public abstract class APatcher<TP>(string name) : InstanceObject<TP> where TP : APatcher<TP>, new() {
    public string Category { get; } = $"{name}PatcherCategory";
    private string PatcherId { get; } = $"com.{name.ToLower()}.patch";
    private bool IsActive { get; set; }
    protected abstract bool DefaultState { get; }
    protected abstract bool Enabled { get; }

    private Harmony m_harmony;
    protected List<MethodToPatch> MethodInfos { get; } = [];

    public static readonly HarmonyMethod PrefixAllow = typeof(APatcher<TP>).GetHarmonyMethod(nameof(myPrefixAllow));
    public static readonly HarmonyMethod PrefixBlock = typeof(APatcher<TP>).GetHarmonyMethod(nameof(myPrefixBlock));
    public static readonly HarmonyMethod PostfixTrue = typeof(APatcher<TP>).GetHarmonyMethod(nameof(myPostfixTrue));
    public static readonly HarmonyMethod PostfixFalse = typeof(APatcher<TP>).GetHarmonyMethod(nameof(myPostfixFalse));
    public static readonly HarmonyMethod PostfixEmpty = typeof(APatcher<TP>).GetHarmonyMethod(nameof(myPostEmpty));

    private static bool myPrefixAllow() => true;

    private static bool myPrefixBlock() => false;

    private static void myPostfixFalse(ref bool result) => result = false;

    private static void myPostfixTrue(ref bool result) => result = true;

    private static void myPostEmpty() {
    }

    protected override void OnInit() {
        m_harmony = new Harmony(PatcherId);
        Patch(DefaultState);
    }

    public void Toggle() => Patch(!IsActive);

    public void Activate() => Patch(true);

    public void Disable() => Patch(false);

    protected virtual void Patch(bool enable = false) {
        if (!Enabled || IsActive == enable) {
            return;
        }

        foreach (MethodToPatch methodToPatch in MethodInfos) {
            MethodBase methodBase = methodToPatch?.ToPatch;
            if (methodBase is null) {
                continue;
            }

            m_harmony.Unpatch(methodBase, HarmonyPatchType.All, PatcherId);
            if (enable) {
                m_harmony.Patch(methodBase, methodToPatch.Prefix, methodToPatch.Postfix);
            }
        }
        IsActive = enable;
    }

    protected void AddMethod(MethodBase methodBase, HarmonyMethod prefix, HarmonyMethod postfix) => MethodInfos.Add(new MethodToPatch(methodBase, prefix, postfix));

    protected void AddMethod(Type t, string method, HarmonyMethod prefix, HarmonyMethod postfix) => AddMethod(t.GetHarmonyMethod(method).method, prefix, postfix);

    protected void AddMethod<T>(string method, HarmonyMethod prefix, HarmonyMethod postfix) => AddMethod(typeof(T), method, prefix, postfix);

    protected void AddMethod(MethodBase methodBase, HarmonyMethod postfix, bool allow = false) => AddMethod(methodBase, allow ? PrefixAllow : PrefixBlock, postfix);

    protected void AddMethod(Type t, string method, HarmonyMethod postfix, bool allow = false) => AddMethod(t.GetHarmonyMethod(method).method, postfix, allow);

    protected void AddMethod<T>(string method, HarmonyMethod postfix, bool allow = false) => AddMethod(typeof(T), method, postfix, allow);

    protected class MethodToPatch(MethodBase toPatch, HarmonyMethod prefix, HarmonyMethod postfix) {
        public MethodBase ToPatch { get; } = toPatch;
        public HarmonyMethod Prefix { get; } = prefix;
        public HarmonyMethod Postfix { get; } = postfix;
    }
}