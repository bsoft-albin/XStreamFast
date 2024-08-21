namespace XStreamFast.Frameworks.CommonMeths
{
    public static class XStreamFastLoggers
    {
        static private String _contentRootPath = String.Empty;
        public static void Initialize(string contentPath)
        {
            _contentRootPath = contentPath;
        }

        /// <summary>
        /// Custom ErrorLog Method for writing Exceptions in the text file.
        /// </summary>
        /// <param name="message">Takes the message from the Exception cause.</param>
        /// <param name="exception">Gets the Actaul Exception message</param>
        public static async Task WriteExceptionLog(Exception exception, String message = "")
        {
            String logFolderPathWithMonthAndyear = Path.Combine(Path.Combine(_contentRootPath, "ErrorLogs"), DateTime.UtcNow.ToString("yyyy-MM"));
            // Check if the subfolder exists, create it if not
            if (!Directory.Exists(logFolderPathWithMonthAndyear))
            {
                Directory.CreateDirectory(logFolderPathWithMonthAndyear);
            }
            String FinallogFilePath = Path.Combine(logFolderPathWithMonthAndyear, $"Errorlog-{DateTime.UtcNow:dd-MM-yyyy}.log");

            try
            {
                using StreamWriter writer = new(FinallogFilePath, append: true);
                await writer.WriteLineAsync($"[Error Timestamp: {DateTime.UtcNow:dd-MM-yyyy} {DateTime.UtcNow.Hour.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + " " + DateTime.UtcNow.ToString("tt", System.Globalization.CultureInfo.InvariantCulture)}]");
                await writer.WriteLineAsync($"Message: {message}");
                await writer.WriteLineAsync($"Exception: {exception.Message}");
                await writer.WriteLineAsync($"Source: {exception.Source}");
                await writer.WriteLineAsync($"StackTrace: {exception.StackTrace}");
                await writer.WriteLineAsync($"TargetSite: {exception.TargetSite}");

                await writer.WriteLineAsync(new String('-', 200)); // Separator for readability
            }
            catch (Exception ex)
            {
                await Task.Run(() => Console.WriteLine(ex));
            }
        }

        public static async Task WriteApiRequestLog(string method, string endpoint, string header, string body, string message = "")
        {
            String logFolderPathWithMonthAndyear = Path.Combine(Path.Combine(_contentRootPath, "ApiCallLogs\\Requests"), DateTime.UtcNow.ToString("yyyy-MM"));
            // Check if the subfolder exists, create it if not
            if (!Directory.Exists(logFolderPathWithMonthAndyear))
            {
                Directory.CreateDirectory(logFolderPathWithMonthAndyear);
            }
            String FinallogFilePath = Path.Combine(logFolderPathWithMonthAndyear, $"ApiRequestLog-{DateTime.UtcNow:dd-MM-yyyy}.log");

            try
            {
                using StreamWriter writer = new(FinallogFilePath, append: true);
                await writer.WriteLineAsync($"[ApiRequestCall Timestamp: {DateTime.UtcNow:dd-MM-yyyy} {DateTime.UtcNow.Hour.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + " " + DateTime.UtcNow.ToString("tt", System.Globalization.CultureInfo.InvariantCulture)}]");
                await writer.WriteLineAsync($"Message: {message}");
                await writer.WriteLineAsync($"Method: {method}");
                await writer.WriteLineAsync($"Endpoint: {endpoint}");
                await writer.WriteLineAsync($"Header: {header}");
                await writer.WriteLineAsync($"Request Body: {body}");

                await writer.WriteLineAsync(new String('-', 200)); // Separator for readability
            }
            catch (Exception ex)
            {
                await Task.Run(() => Console.WriteLine(ex));
            }
        }

        public static async Task WriteApiResponseLog(string method, string endpoint, string header, string body, int statuscode, string message = "")
        {
            String logFolderPathWithMonthAndyear = Path.Combine(Path.Combine(_contentRootPath, "ApiCallLogs\\Responses"), DateTime.UtcNow.ToString("yyyy-MM"));
            // Check if the subfolder exists, create it if not
            if (!Directory.Exists(logFolderPathWithMonthAndyear))
            {
                Directory.CreateDirectory(logFolderPathWithMonthAndyear);
            }
            String FinallogFilePath = Path.Combine(logFolderPathWithMonthAndyear, $"ApiResponseLog-{DateTime.UtcNow:dd-MM-yyyy}.log");

            try
            {
                using StreamWriter writer = new(FinallogFilePath, append: true);
                await writer.WriteLineAsync($"[ApiRequestCall Timestamp: {DateTime.UtcNow:dd-MM-yyyy} {DateTime.UtcNow.Hour.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + " " + DateTime.UtcNow.ToString("tt", System.Globalization.CultureInfo.InvariantCulture)}]");
                await writer.WriteLineAsync($"Message: {message}");
                await writer.WriteLineAsync($"Method: {method}");
                await writer.WriteLineAsync($"Endpoint: {endpoint}");
                await writer.WriteLineAsync($"Header: {header}");
                await writer.WriteLineAsync($"Response Body: {body}");
                await writer.WriteLineAsync($"Response StatusCode: {statuscode}");

                await writer.WriteLineAsync(new String('-', 200)); // Separator for readability
            }
            catch (Exception ex)
            {
                await Task.Run(() => Console.WriteLine(ex));
            }
        }

    }
}
