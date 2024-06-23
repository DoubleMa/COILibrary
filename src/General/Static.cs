using COILib.Extensions;
using COILib.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace COILib.General {

    public class Static {

        public static string GetCallingAssemblyName() => GetCallingAssemblyName(false);

        public static string GetCallingAssemblyLocation(params string[] file) => GetCallingAssemblyLocation(false, file);

        public static string GetCallingAssemblyName(bool current) {
            var assembly = GetCallingAssembly(current);
            if (assembly == null) return null;
            return assembly.GetName().Name;
        }

        internal static string GetCallingAssemblyLocation(bool current = false, params string[] file) {
            var assembly = GetCallingAssembly(current);
            if (assembly == null) return null;
            var path = Path.GetDirectoryName(assembly.Location);
            if (path == null) return null;
            return file != null ? Path.Combine(file.UnShift(path)) : path;
        }

        internal static Assembly GetCallingAssembly(bool current) {
            var executingAssembly = Assembly.GetExecutingAssembly();
            if (current) return executingAssembly;
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

        public static async void CopyFilesAsync(string sourceDir, string targetDir, params string[] ignoreFiles) {
            await TryRunTask(() => CopyFilesRec(sourceDir, targetDir, ignoreFiles));
        }

        public static async Task CopyFilesRec(string sourceDir, string targetDir, params string[] ignoreFiles) {
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
            var files = Directory.GetFiles(sourceDir);
            foreach (var file in files) {
                var fileName = Path.GetFileName(file);
                if (ignoreFiles != null && ignoreFiles.Contains(fileName)) continue;
                var destFile = Path.Combine(targetDir, fileName);

                bool copied = false;
                int retries = 3;
                while (!copied && retries > 0) {
                    try {
                        File.Copy(file, destFile, true);
                        copied = true;
                    }
                    catch when (retries > 0) {
                        retries--;
                        Task.Delay(1000).Wait();
                    }
                }
                if (!copied) {
                    throw new IOException($"Failed to copy file: {file}");
                }
            }

            var directories = Directory.GetDirectories(sourceDir);
            foreach (var directory in directories) {
                var dirName = Path.GetFileName(directory);
                if (ignoreFiles != null && ignoreFiles.Contains(dirName)) continue;
                var destDir = Path.Combine(targetDir, dirName);
                await CopyFilesRec(directory, destDir, ignoreFiles);
            }
        }

        public static async void DeleteFilesAsync(string targetDir, params string[] ignoreFiles) {
            await TryRunTask(() => DeleteFilesRec(targetDir, ignoreFiles));
        }

        public static async Task DeleteFilesRec(string targetDir, params string[] ignoreFiles) {
            if (!Directory.Exists(targetDir)) return;
            var files = Directory.GetFiles(targetDir);
            var deleteDir = true;
            foreach (var file in files) {
                var fileName = Path.GetFileName(file);
                if (ignoreFiles != null && ignoreFiles.Contains(fileName)) {
                    deleteDir = false;
                    continue;
                }
                File.Delete(file);
            }
            var directories = Directory.GetDirectories(targetDir);
            foreach (var directory in directories) {
                var dirName = Path.GetFileName(directory);
                if (ignoreFiles != null && ignoreFiles.Contains(dirName)) {
                    deleteDir = false;
                    continue;
                }
                await DeleteFilesRec(directory, ignoreFiles);
            }
            if (deleteDir) Directory.Delete(targetDir);
        }
    }
}