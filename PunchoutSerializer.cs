using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
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
        /// Parse from an Http body string
        /// </summary>
        /// <param name="httpBody"></param>
        /// <returns></returns>
        public static Punchout Deserialize(string httpBody)
        {
            var dict = new Dictionary<string, string>();
            foreach (var keyValue in httpBody.Split('&'))
            {
                var trimmed = HttpUtility.UrlDecode(keyValue.Trim('\r', '\n'));
                var split = trimmed.Split('=');
                if (split.Length != 2) continue;

                dict.Add(split[0], split[1]);
            }

            return Deserialize(dict);
        }

        /// <summary>
        /// Parse from an Http body stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Punchout Deserialize(Stream stream)
        {
            using var sr = new StreamReader(stream);
            return Deserialize(sr.ReadToEnd());
        }

        /// <summary>
        /// Parse from an ASP.NET IFormCollection (Request.Forms)
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Punchout Deserialize(IFormCollection form)
        {
            var dict = new Dictionary<string, string>();
            foreach (var keyValue in form)
            {
                dict.Add(keyValue.Key, keyValue.Value);
            }
            return Deserialize(dict);
        }

        /// <summary>
        /// Parse from an enumerable KeyValuePair or Dictionary
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public static Punchout Deserialize(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            var punchout = new Punchout();
            foreach (var keyValue in keyValuePairs)
            {
                foreach (var property in PunchoutEntry.GetPublicProperties())
                {
                    var pattern = Regex.Escape(property.GetCustomAttribute<FieldNameAttribute>()?.Name ?? "").Replace("n", @"(\d)");
                    if (string.IsNullOrWhiteSpace(pattern)) continue;

                    var match = Regex.Match(keyValue.Key, pattern);

                    if (!match.Success) continue;

                    var id = int.Parse(match.Groups[1].Value);
                    var entry = punchout.Entries.FirstOrDefault(e => e.Id == id) ?? new PunchoutEntry();
                    entry.Id = id;

                    property.SetValue(entry, ParseValue(keyValue.Value, property));

                    // Update the entry
                    punchout.Entries.Remove(entry);
                    punchout.Entries.Add(entry);
                }
            }

            return punchout;
        }

        /// <summary>
        /// Serialize the punchout entries to a http body string
        /// </summary>
        /// <param name="punchout">The punchout from which the entries should be serialized</param>
        /// <returns></returns>
        public static string Serialize(Punchout punchout) => Serialize(punchout.Entries);

        /// <summary>
        /// Serialize the punchout entries to a http body string
        /// </summary>
        /// <param name="entries">The entries to serialize</param>
        /// <returns></returns>
        public static string Serialize(IEnumerable<PunchoutEntry> entries)
        {
            if (!entries.Any()) return string.Empty;

            string str = "";

            foreach (var entry in entries)
            {
                // Get all public properties
                PropertyInfo[] properties = PunchoutEntry.GetPublicProperties();

                foreach (PropertyInfo? property in properties)
                {
                    var key = property.GetCustomAttribute<FieldNameAttribute>()?.Name.Replace("n", entry.Id.ToString());
                    var value = property.GetValue(entry);

                    if (key == null || value == null) continue;

                    var stringValue = SerializeValue(value);
                    if (stringValue == null) continue;
                    

                    str += "NEW_ITEM-" + key + "=" + HttpUtility.UrlEncodeUnicode(stringValue) + '&';
                }
            }

            // Remove the last trailing '&'
            return str.Remove(str.Length - 1);
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
