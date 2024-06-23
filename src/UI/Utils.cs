using COILib.General;
using COILib.Logging;
using Mafi;
using Mafi.Unity;
using Mafi.Unity.UiToolkit.Component;
using Mafi.Unity.UiToolkit.Library;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace COILib.UI;

public class Utils {

    public static Img GetImage(string pathOrUrl, string defaultAssets = Assets.Unity.UserInterface.General.Empty128_png) {
        Img img = new(defaultAssets);
        if (!string.IsNullOrEmpty(pathOrUrl)) {
            UpdateImageOnMainThread(img, pathOrUrl);
        }

        return img;
    }

    public static Img GetImage(string path, Px width, Px height) => GetImage(path).Size(width, height);

    public static Img GetImage(string path, string defaultAssets, Px width, Px height) => GetImage(path, defaultAssets).Size(width, height);

    private static async void UpdateImageOnMainThread(Img img, string pathOrUrl) {
        try {
            byte[] imgData = await Task.Run(() => getImageBytes(pathOrUrl));
            if (imgData != null) {
                RunOnMainThread(() => updateImage(img, imgData));
            }
        }
        catch (Exception ex) {
            ExtLog.Error($"Error updating image from {pathOrUrl}: {ex.Message}");
        }
    }

    private static void RunOnMainThread(Action action) {
        var context = SynchronizationContext.Current;
        if (context != null) {
            context.Post(_ => action(), null);
        }
        else {
            action();
        }
    }

    private static byte[] getImageBytes(string path) {
        try {
            if (File.Exists(path)) {
                return File.ReadAllBytes(path);
            }

            if (Uri.IsWellFormedUriString(path, UriKind.Absolute)) {
                using HttpClient client = new();
                return client.GetByteArrayAsync(path).Result;
            }
        }
        catch (Exception ex) {
            ExtLog.Error($"Error getting image bytes from {path}: {ex.Message}");
        }
        return null;
    }

    private static void updateImage(Img img, byte[] imgData) {
        Static.TryRun(() => {
            if (img == null || imgData == null) {
                return;
            }

            Texture2D texture = new(width: 2, height: 2);
            if (texture.LoadImage(imgData)) {
                img.Image(texture);
                img.RootElement.MarkDirtyRepaint();
            }
            else {
                ExtLog.Error("Failed to load texture from image data");
            }
        });
    }
}