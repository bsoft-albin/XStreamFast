namespace XStreamFast.Frameworks.CommonProps
{
    public class XStreamFastApiRoutes
    {

        public static class Templates
        {
            public const String API_TEMPLATE = "api/[controller]/[action]";
            public const String ApiVersionTemplate = "api/v{version:apiVersion}/[controller]/[action]";
        }

        public static class ActionNames
        {
            public const String SayHello = "SayHelloAsync";
            public const String GetReligions = "GetReligionsAsync";
        }

        public static class Versions
        {
            public const String DEFAULT = "1.0";
            public const String LongTermVersion = "2.0";
            public const String Latest = "3.0";
        }
    }
}
