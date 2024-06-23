using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COILib.General;

namespace COILib.Helper {

    public static class FileHelper {

        public static async void CopyFilesAsync(string sourceDir, string targetDir, params string[] ignoreFiles) {
            await Static.TryRunTask(() => CopyFilesRec(sourceDir, targetDir, ignoreFiles));
        }

        public static async Task CopyFilesRec(string sourceDir, string targetDir, params string[] ignoreFiles) {
            if (!Directory.Exists(targetDir)) {
                Directory.CreateDirectory(targetDir);
            }

            string[] files = Directory.GetFiles(sourceDir);
            foreach (string file in files) {
                string fileName = Path.GetFileName(file);
                if (ignoreFiles != null && ignoreFiles.Contains(fileName)) {
                    continue;
                }

                string destFile = Path.Combine(targetDir, fileName);

                bool copied = false;
                int retries = 3;
                while (!copied && retries > 0) {
                    try {
                        File.Copy(file, destFile, true);
                        copied = true;
                    }
                    catch {
                        retries--;
                        Task.Delay(1000).Wait();
                    }
                }
                if (!copied) {
                    throw new IOException($"Failed to copy file: {file}");
                }
            }

            string[] directories = Directory.GetDirectories(sourceDir);
            foreach (string directory in directories) {
                string dirName = Path.GetFileName(directory);
                if (ignoreFiles != null && ignoreFiles.Contains(dirName)) {
                    continue;
                }

                string destDir = Path.Combine(targetDir, dirName);
                await CopyFilesRec(directory, destDir, ignoreFiles);
            }
        }

        public static async void DeleteFilesAsync(string targetDir, params string[] ignoreFiles) {
            await Static.TryRunTask(() => DeleteFilesRec(targetDir, ignoreFiles));
        }

        public static async Task DeleteFilesRec(string targetDir, params string[] ignoreFiles) {
            if (!Directory.Exists(targetDir)) {
                return;
            }

            string[] files = Directory.GetFiles(targetDir);
            bool deleteDir = true;
            foreach (string file in files) {
                string fileName = Path.GetFileName(file);
                if (ignoreFiles != null && ignoreFiles.Contains(fileName)) {
                    deleteDir = false;
                    continue;
                }
                File.Delete(file);
            }

            string[] directories = Directory.GetDirectories(targetDir);
            foreach (string directory in directories) {
                string dirName = Path.GetFileName(directory);
                if (ignoreFiles != null && ignoreFiles.Contains(dirName)) {
                    deleteDir = false;
                    continue;
                }
                await DeleteFilesRec(directory, ignoreFiles);
            }

            if (deleteDir) {
                Directory.Delete(targetDir);
            }
        }
    }
}