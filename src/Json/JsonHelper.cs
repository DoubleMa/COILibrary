using COILib.General;
using Newtonsoft.Json;
using System.IO;

namespace COILib.Json {

    public static class JsonHelper {

        public static string Serialize<T>(T toSerialize, string path = null, bool format = false) {
            string result = Static.TryRun(() => JsonConvert.SerializeObject(toSerialize, format ? Formatting.Indented : Formatting.None), "{}");
            if (path != null) Static.TryRun(() => File.WriteAllText(path, result));
            return result;
        }

        public static T Deserialize<T>(string path, T defValue = default) => Static.TryRun(() => File.Exists(path) ? JsonConvert.DeserializeObject<T>(File.ReadAllText(path)) : defValue, defValue);
    }

    public class SerializableObject<T> where T : class, new() {

        public string Json(string path = null, bool format = true) => JsonHelper.Serialize(this, path, format);

        public static T Deserialize(string path, T defValue = default) => JsonHelper.Deserialize(path, defValue);

        public static T DeserializeNew(string path) => JsonHelper.Deserialize(path, new T());
    }
}