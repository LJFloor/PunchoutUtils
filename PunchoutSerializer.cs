using Microsoft.AspNetCore.Http;
using PunchoutUtils.Attributes;
using PunchoutUtils.Models;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;

namespace PunchoutUtils
{
    public class PunchoutSerializer
    {
        /// <summary>
        /// Parse from an http body string to the default PunchoutEntry model
        /// </summary>
        /// <param name="httpBody"></param>
        /// <returns></returns>
        public static IEnumerable<PunchoutEntry> Deserialize(string httpBody) => Deserialize<PunchoutEntry>(httpBody);

        /// <summary>
        /// Parse from an Http body stream to the default PunchoutEntry mod
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IEnumerable<PunchoutEntry> Deserialize(Stream stream) => Deserialize<PunchoutEntry>(stream);

        /// <summary>
        /// Parse from an ASP.NET IFormCollection (Request.Forms) to the default PunchoutEntry model
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static IEnumerable<PunchoutEntry> Deserialize(IFormCollection form) => Deserialize<PunchoutEntry>(form);

        /// <summary>
        /// Parse from an enumerable KeyValuePair or Dictionary to the default PunchoutEntry model
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public static IEnumerable<PunchoutEntry> Deserialize(IEnumerable<KeyValuePair<string, string>> keyValuePairs) => Deserialize<PunchoutEntry>(keyValuePairs);

        /// <summary>
        /// Parse from an Http body string
        /// </summary>
        /// <param name="httpBody"></param>
        /// <returns></returns>
        public static IEnumerable<T> Deserialize<T>(string httpBody) where T : new()
        {
            var dict = new Dictionary<string, string>();
            foreach (var keyValue in httpBody.Split('&'))
            {
                var trimmed = HttpUtility.UrlDecode(keyValue.Trim('\r', '\n'));
                var split = trimmed.Split('=');
                if (split.Length != 2) continue;

                dict.Add(split[0], split[1]);
            }

            return Deserialize<T>(dict);
        }

        /// <summary>
        /// Parse from an Http body stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IEnumerable<T> Deserialize<T>(Stream stream) where T : new()
        {
            using var sr = new StreamReader(stream);
            return Deserialize<T>(sr.ReadToEnd());
        }

        /// <summary>
        /// Parse from an ASP.NET IFormCollection (Request.Forms)
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static IEnumerable<T> Deserialize<T>(IFormCollection form) where T : new()
        {
            var dict = new Dictionary<string, string>();
            foreach (var keyValue in form)
            {
                dict.Add(keyValue.Key, keyValue.Value);
            }
            return Deserialize<T>(dict);
        }

        /// <summary>
        /// Parse from an enumerable KeyValuePair or Dictionary
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public static IEnumerable<T> Deserialize<T>(IEnumerable<KeyValuePair<string, string>> keyValuePairs) where T : new()
        {
            var entries = new Dictionary<int, T>();
            foreach (var keyValue in keyValuePairs)
            {
                // Get all public properties
                PropertyInfo[] properties = typeof(T).GetProperties().Where(p => p.GetMethod?.IsPublic ?? false).ToArray();

                foreach (var property in properties)
                {
                    var pattern = "NEW_ITEM-" + Regex.Escape(property.GetCustomAttribute<FieldNameAttribute>()?.Name ?? Guid.NewGuid().ToString()).Replace("n", @"(\d)");
                    if (string.IsNullOrWhiteSpace(pattern)) continue;

                    var match = Regex.Match(keyValue.Key, pattern);

                    if (!match.Success) continue;

                    var id = int.Parse(match.Groups[1].Value);

                    // Get the keyvalue with the specified id, or create a new one if it doesn't exist.
                    var entry = entries.FirstOrDefault(e => e.Key == id);
                    if (entry.Value == null)
                    {
                        entry = new KeyValuePair<int, T>(id, new T());
                    }

                    property.SetValue(entry.Value, ParseValue(keyValue.Value, property));

                    // Update the entry
                    entries.Remove(entry.Key);
                    entries.Add(entry.Key, entry.Value);
                }
                
            }

            return entries.Values;
        }

        /// <summary>
        /// Serialize an entry to a http body string
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string Serialize(object entry) => Serialize(new object[] { entry });

        /// <summary>
        /// Serialize the punchout entries to a http body string
        /// </summary>
        /// <param name="entries">The entries to serialize</param>
        /// <returns></returns>
        public static string Serialize(IEnumerable<object> entries)
        {
            if (!entries.Any()) return string.Empty;

            string str = "";

            int i = 0;
            foreach (object? entry in entries)
            {
                // Get all public properties
                PropertyInfo[] properties = entry.GetType().GetProperties().Where(p => p.GetMethod?.IsPublic ?? false).ToArray();

                foreach (PropertyInfo? property in properties)
                {
                    var key = property.GetCustomAttribute<FieldNameAttribute>()?.Name.Replace("n", i.ToString());
                    var value = property.GetValue(entry);

                    if (key == null || value == null) continue;

                    var stringValue = SerializeValue(value);
                    if (stringValue == null) continue;

                    str += HttpUtility.UrlEncodeUnicode("NEW_ITEM-" + key) + '=' + HttpUtility.UrlEncodeUnicode(stringValue) + '&';
                }
                i++;
            }

            // Remove the last trailing '&'
            if (str.Length > 1)
            {
                str = str.Remove(str.Length - 1);
            }

            return str;
        }

        private static string? SerializeValue(object value)
        {
            if (value == null) return null;
            var type = value.GetType().IsGenericType ? value.GetType().GenericTypeArguments[0] : value.GetType();

            if (type.IsEnum)
            {
                // Get the defined EnumMember Value (or the name of the enum member if null)
                return value.GetType()
                    .GetTypeInfo()
                    .DeclaredMembers
                    .SingleOrDefault(e => e.Name == value.ToString())
                    ?.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? value.ToString();
            }

            else if (type == typeof(bool))
            {
                return (bool)value ? "X" : null;
            }

            return value.ToString();
        }

        private static object? ParseValue(string value, PropertyInfo property)
        {
            var type = property.PropertyType.IsGenericType ? property.PropertyType.GenericTypeArguments[0] : property.PropertyType;

            // Don't bother with empty values, except for flags (booleans)
            if (string.IsNullOrWhiteSpace(value) && type != typeof(bool)) return null;

            if (type == typeof(string))
            {
                return value;
            }

            else if (type == typeof(int))
            {
                // We first parse to a float, so a value like 2.00 will still be recognized and cut off to 2
                return (int)float.Parse(value);
            }

            else if (type == typeof(float))
            {
                return float.Parse(value);
            }

            else if (type == typeof(Uri))
            {
                return new Uri(value);
            }

            else if (type == typeof(bool))
            {
                return value.ToString().ToUpper() == "X";
            }

            else if (type == typeof(char))
            {
                return char.Parse(value);
            }

            else if (type == typeof(uint))
            {
                return uint.Parse(value);
            }

            else if (type == typeof(short))
            {
                return short.Parse(value);
            }

            else if (type == typeof(ushort))
            {
                return ushort.Parse(value);
            }

            else if (type == typeof(long))
            {
                return long.Parse(value);
            }

            else if (type == typeof(ulong))
            {
                return ulong.Parse(value);
            }

            else if (type.IsEnum)
            {
                // https://stackoverflow.com/questions/27372816/how-to-read-the-value-for-an-enummember-attribute

                var enumName = type
                    .GetTypeInfo()
                    .DeclaredMembers
                    .SingleOrDefault(x => x.Name == value || x.GetCustomAttribute<EnumMemberAttribute>()?.Value == value)
                    ?.Name;

                if (enumName == null) return null;
                return Enum.Parse(type, enumName, true);
            }

            return null;
        }
    }
}
