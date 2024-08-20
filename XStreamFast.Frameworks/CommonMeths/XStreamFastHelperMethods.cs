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
    }
    
}
