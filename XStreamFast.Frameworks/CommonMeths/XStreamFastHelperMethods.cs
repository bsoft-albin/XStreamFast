using Newtonsoft.Json;
using System.Xml.Serialization;

namespace XStreamFast.Frameworks.CommonMeths
{
    public static class HelperMeths {
        public static string SerializeToXml(object data)
        {
            XmlSerializer serializer = new(data.GetType());
            using (StringWriter stringWriter = new())
            {
                serializer.Serialize(stringWriter, data);
                return stringWriter.ToString();
            }
        }

        public static string SerializeToJson(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public static string ReadHtmlFileAsString(string baseWWWRootPath, string relativePath)
        {
            // Example of using the path
            string filePath = Path.Combine(baseWWWRootPath, relativePath);

            // Read the file or perform any other operation
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string ReplaceLastSegment(string path, string newSegment)
        {
            // Ensure the path is not null or empty
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path cannot be null or empty", nameof(path));

            if (string.IsNullOrEmpty(newSegment)) throw new ArgumentException("Path cannot be null or empty", nameof(newSegment));

            // Get the directory of the path, which excludes the last segment
            string directory = Path.GetDirectoryName(path) ?? "";

            // Combine the directory with the new segment
            string newPath = Path.Combine(directory ?? string.Empty, newSegment);

            return newPath;
        }
    }

}
