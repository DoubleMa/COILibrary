using COILib.General;
using System;

namespace COILib.Logging;

public static class ExtLog {

    private static string addPrefix(this string message, string prefix = null) => $"[{prefix ?? Static.GetCallingAssemblyName()}]: {message}";

    private static void log(object message, Action<string> callback = null, string prefix = null) {
        string prefixedMessage = message?.ToString().addPrefix(prefix);
        Console.WriteLine(prefixedMessage);
        callback?.Invoke(prefixedMessage);
    }

    private static void log(Exception ex, object message, Action<Exception, string> callback = null, string prefix = null) {
        string prefixedMessage = message?.ToString().addPrefix(prefix);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{prefixedMessage}\n{ex.StackTrace}");
        Console.ResetColor();
        callback?.Invoke(ex, prefixedMessage);
    }

    public static void Info(object message, string prefix = null) => log(message, Mafi.Log.Info, prefix);

    internal static void Info(object message, bool current) => Info(message, Static.GetCallingAssemblyName(current));

    public static void InfoDebug(object message, string prefix = null) => log(message, Mafi.Log.InfoDebug, prefix);

    internal static void InfoDebug(object message, bool current) => InfoDebug(message, Static.GetCallingAssemblyName(current));

    public static void Warning(object message, string prefix = null) => log(message, Mafi.Log.Warning, prefix);

    internal static void Warning(object message, bool current) => Warning(message, Static.GetCallingAssemblyName(current));

    public static void WarningOnce(object message, string prefix = null) => log(message, Mafi.Log.WarningOnce, prefix);

    internal static void WarningOnce(object message, bool current) => WarningOnce(message, Static.GetCallingAssemblyName(current));

    public static void Error(object message, string prefix = null) => log(message, Mafi.Log.Error, prefix);

    internal static void Error(object message, bool current) => Error(message, Static.GetCallingAssemblyName(current));

    public static void Exception(Exception e, object message, string prefix = null) => log(e, message, Mafi.Log.Exception, prefix);

    internal static void Exception(Exception e, object message, bool current) => Exception(e, message, Static.GetCallingAssemblyName(current));
}