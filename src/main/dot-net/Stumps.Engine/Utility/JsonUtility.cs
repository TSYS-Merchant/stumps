namespace Stumps.Utility {

    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    internal static class JsonUtility {

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            Formatting = Formatting.Indented
        };

        public static IList<T> DeserializeFromDirectory<T>(string path, string searchPattern, SearchOption searchOption) {

            var list = new List<T>();
            var files = Directory.GetFiles(path, searchPattern, searchOption);

            foreach ( var file in files ) {

                var value = JsonUtility.DeserializeFromFile<T>(file);
                list.Add(value);

            }

            return list;

        }

        public static T DeserializeFromFile<T>(string path) {

            var json = File.ReadAllText(path);
            var value = JsonConvert.DeserializeObject<T>(json);

            return value;

        }

        public static void SerializeToFile(object value, string path) {

            var json = JsonConvert.SerializeObject(value, Settings);
            File.WriteAllText(path, json);

        }

    }

}
