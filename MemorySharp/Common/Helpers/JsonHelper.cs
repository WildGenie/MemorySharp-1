using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Binarysharp.MemoryManagement.Common.Helpers
{
    /// <summary>
    ///     Provides methods for Serialization and Deserialization of JSON/JavaScript Object Notation documents.
    /// </summary>
    public static class JsonHelper
    {
        #region Public Methods
        /// <summary>
        ///     Serializes the specified object and writes the Json document to the specified path.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="path">The path where the file is saved.</param>
        /// <param name="encoding">The encoding to generate.</param>
        public static void ExportToFile<T>(T obj, string path, Encoding encoding)
        {
            // Create the stream to write into the specified file
            var temp = JsonConvert.SerializeObject(obj, Formatting.Indented);
            using (var file = File.Open(path, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(temp);
                }
            }
        }

        /// <summary>
        ///     Serializes the specified object and writes the Json document to the specified path using
        ///     <see cref="Encoding.UTF8" /> encoding.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="path">The path where the file is saved.</param>
        public static void ExportToFile<T>(T obj, string path)
        {
            ExportToFile(obj, path, Encoding.UTF8);
        }

        /// <summary>
        ///     Serializes the specified object and returns the Json document.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>Json document of the serialized object.</returns>
        public static string ExportToString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        /// <summary>
        ///     Deserializes the specified file into an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="path">The path where the object is read.</param>
        /// <param name="encoding">The character encoding to use. </param>
        /// <returns>The deserialized object.</returns>
        public static T ImportFromFile<T>(string path, Encoding encoding)
        {
            using (var sr = new StreamReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
            }
        }

        /// <summary>
        ///     Deserializes the specified file into an object using <see cref="Encoding.UTF8" /> encoding.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="path">The path where the object is read.</param>
        /// <returns>The deserialized object.</returns>
        public static T ImportFromFile<T>(string path)
        {
            return ImportFromFile<T>(path, Encoding.UTF8);
        }

        /// <summary>
        ///     Deserializes the Json document to the specified object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="serializedObj">The string representing the serialized object.</param>
        /// <returns>The deserialized object.</returns>
        public static T ImportFromString<T>(string serializedObj)
        {
            using (var stringReader = new StringReader(serializedObj))
            {
                // Return the serialized object
                return JsonConvert.DeserializeObject<T>(stringReader.ReadToEnd());
            }
        }
        #endregion
    }
}