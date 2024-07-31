namespace XStreamFast.Frameworks.CommonProps
{
    public class AppProps
    {
        public static class Startup
        {
            public const String CORS_POLICY = "AllowAnyOrigin";
        }

        public static class Configuration
        {
            public const String SQL_Server_Connection_String = "WorkConnString";
            public const String MySql_Connection_String = "MysqlConnString";
            public const String Postgres_Connection_String = "PostgresConnString";
        }
    }
}
