namespace Stumps.Utility
{

    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    ///     A class that represents a set of JSON related functions.
    /// </summary>
    internal static class JsonUtility
    {

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            Formatting = Formatting.Indented
        };

        /// <summary>
        ///     Deserializes all objects of a specified type from a directory.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="path">The path of the directory.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns>
        ///     A generic list of objects deserialized from the specified directory.
        /// </returns>
        public static IList<T> DeserializeFromDirectory<T>(string path, string searchPattern, SearchOption searchOption)
        {

            var list = new List<T>();
            var files = Directory.GetFiles(path, searchPattern, searchOption);

            foreach (var file in files)
            {

                var value = JsonUtility.DeserializeFromFile<T>(file);
                list.Add(value);

            }

            return list;

        }

        /// <summary>
        ///     Deserializes an object from a file.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="path">The path of the file to deserialize.</param>
        /// <returns>
        ///     The deserialized form of the specified file.
        /// </returns>
        public static T DeserializeFromFile<T>(string path)
        {

            var json = File.ReadAllText(path);
            var value = JsonConvert.DeserializeObject<T>(json);

            return value;

        }

        /// <summary>
        ///     Serializes the specified object to a file.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="path">The path to the file.</param>
        public static void SerializeToFile(object value, string path)
        {

            var json = JsonConvert.SerializeObject(value, Settings);
            File.WriteAllText(path, json);

        }

    }

}