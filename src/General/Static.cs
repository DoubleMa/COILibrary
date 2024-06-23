using COILib.Extensions;
using COILib.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace COILib.General;

public class Static {

    public static string GetCallingAssemblyName() => GetCallingAssemblyName(false);

    public static string GetCallingAssemblyLocation(params string[] file) => GetCallingAssemblyLocation(false, file);

    public static string GetCallingAssemblyName(bool current) {
        var assembly = getCallingAssembly(current);
        return assembly?.GetName().Name;
    }

    internal static string GetCallingAssemblyLocation(bool current = false, params string[] file) {
        var assembly = getCallingAssembly(current);
        if (assembly == null) {
            return null;
        }

        string path = Path.GetDirectoryName(assembly.Location);
        if (path == null) {
            return null;
        }

        return file != null ? Path.Combine(file.UnShift(path)) : path;
    }

    private static Assembly getCallingAssembly(bool current) {
        var executingAssembly = Assembly.GetExecutingAssembly();
        if (current) {
            return executingAssembly;
        }

        // Possible null reference exception
        foreach (var frame in new StackTrace().GetFrames()) {
            var method = frame.GetMethod();
            var assembly = method?.DeclaringType?.Assembly;
            if (assembly != null && assembly != executingAssembly) {
                return assembly;
            }
        }
        return null;
    }

    public static async Task<T> TryRunTask<T>(Func<T> func, T defValue = default, Action error = null, Action final = null) {
        return await Task.Run(() => TryRun(func, defValue, error, final));
    }

    public static async Task TryRunTask(Action action, Action error = null, Action final = null) {
        await Task.Run(() => TryRun(action, error, final));
    }

    public static async Task<T> TryRunAsync<T>(Func<Task<T>> func, T defValue = default, Action error = null, Action final = null) {
        try {
            return await func.Invoke();
        }
        catch (Exception ex) {
            ExtLog.Exception(ex, $"Failed to execute {func.Method.Name}: {ex.Message}");
            error?.Invoke();
        }
        finally {
            final?.Invoke();
        }
        return defValue;
    }

    public static async Task TryRunAsync(Func<Task> func, Action error = null, Action final = null) {
        try {
            await func.Invoke();
        }
        catch (Exception ex) {
            ExtLog.Exception(ex, $"Failed to execute {func.Method.Name}: {ex.Message}");
            error?.Invoke();
        }
        finally {
            final?.Invoke();
        }
    }

    public static void TryRun(Action action, Action error = null, Action final = null) {
        try {
            action.Invoke();
        }
        catch (Exception ex) {
            ExtLog.Exception(ex, $"Failed to excute {action.Method.Name}: {ex.Message}");
            error?.Invoke();
        }
        finally {
            final?.Invoke();
        }
    }

    public static T TryRun<T>(Func<T> func, T defValue = default, Action error = null, Action final = null) {
        try {
            return func.Invoke();
        }
        catch (Exception ex) {
            ExtLog.Exception(ex, $"Failed to excute {func.Method.Name}: {ex.Message}");
            error?.Invoke();
        }
        finally {
            final?.Invoke();
        }
        return defValue;
    }
}