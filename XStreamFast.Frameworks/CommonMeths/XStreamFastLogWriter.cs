using XStreamFast.Models;

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
                await writer.WriteLineAsync($"[Error Timestamp : {DateTime.UtcNow:dd-MM-yyyy} {DateTime.UtcNow.Hour.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + " " + DateTime.UtcNow.ToString("tt", System.Globalization.CultureInfo.InvariantCulture)}]");
                await writer.WriteLineAsync($"Message : {message}");
                await writer.WriteLineAsync($"Exception : {exception.Message}");
                await writer.WriteLineAsync($"Source : {exception.Source}");
                await writer.WriteLineAsync($"StackTrace : {exception.StackTrace}");
                await writer.WriteLineAsync($"TargetSite : {exception.TargetSite}");

                await writer.WriteLineAsync(new String('-', 200)); // Separator for readability
            }
            catch (Exception ex)
            {
                await Task.Run(() => Console.WriteLine(ex));
            }
        }

        public static async Task WriteApiRequestLog(StringRequestLog logRequest)
        {
            String logFolderPathWithMonthAndyear = Path.Combine(Path.Combine(_contentRootPath, "ApiRequestResponseLogs\\Requests"), DateTime.UtcNow.ToString("yyyy-MM"));
            // Check if the subfolder exists, create it if not
            if (!Directory.Exists(logFolderPathWithMonthAndyear))
            {
                Directory.CreateDirectory(logFolderPathWithMonthAndyear);
            }
            String FinallogFilePath = Path.Combine(logFolderPathWithMonthAndyear, $"ApiRequestLog-{DateTime.UtcNow:dd-MM-yyyy}.log");

            try
            {
                using StreamWriter writer = new(FinallogFilePath, append: true);
                await writer.WriteLineAsync($"[ApiRequestCall Timestamp : {DateTime.UtcNow:dd-MM-yyyy} {DateTime.UtcNow.Hour.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + " " + DateTime.UtcNow.ToString("tt", System.Globalization.CultureInfo.InvariantCulture)}]");
                await writer.WriteLineAsync($"Message : {logRequest.Message}");
                await writer.WriteLineAsync($"Method : {logRequest.HttpMethod}");
                await writer.WriteLineAsync($"Endpoint : {logRequest.Endpoint}");
                await writer.WriteLineAsync($"Header : {logRequest.Headers}");
                await writer.WriteLineAsync($"Request Body : {logRequest.Body}");
                await writer.WriteLineAsync($"Query Parameters : {logRequest.QueryParameters}");
                await writer.WriteLineAsync($"User Agent Requested : {logRequest.UserAgent}");
                await writer.WriteLineAsync($"User Identity : {logRequest.UserIdentity}");
                await writer.WriteLineAsync($"Client IP Address : {logRequest.ClientIpAddress}");
                await writer.WriteLineAsync($"Process Id : {logRequest.CorrelationId}");

                await writer.WriteLineAsync(new String('-', 200)); // Separator for readability
            }
            catch (Exception ex)
            {
                await Task.Run(() => Console.WriteLine(ex));
            }
        }

        public static async Task WriteApiResponseLog(StringResponseLog stringResponseLog)
        {
            String logFolderPathWithMonthAndyear = Path.Combine(Path.Combine(_contentRootPath, "ApiRequestResponseLogs\\Responses"), DateTime.UtcNow.ToString("yyyy-MM"));
            // Check if the subfolder exists, create it if not
            if (!Directory.Exists(logFolderPathWithMonthAndyear))
            {
                Directory.CreateDirectory(logFolderPathWithMonthAndyear);
            }
            String FinallogFilePath = Path.Combine(logFolderPathWithMonthAndyear, $"ApiResponseLog-{DateTime.UtcNow:dd-MM-yyyy}.log");

            try
            {
                using StreamWriter writer = new(FinallogFilePath, append: true);
                await writer.WriteLineAsync($"[ApiRequestCall Timestamp : {DateTime.UtcNow:dd-MM-yyyy} {DateTime.UtcNow.Hour.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + ":" + DateTime.UtcNow.Minute.ToString() + " " + DateTime.UtcNow.ToString("tt", System.Globalization.CultureInfo.InvariantCulture)}]");
                await writer.WriteLineAsync($"Message : {stringResponseLog.Message}");
                await writer.WriteLineAsync($"Method : {stringResponseLog.HttpMethod}");
                await writer.WriteLineAsync($"Endpoint : {stringResponseLog.Endpoint}");
                await writer.WriteLineAsync($"Header : {stringResponseLog.Headers}");
                await writer.WriteLineAsync($"Response Body : {stringResponseLog.Body}");
                await writer.WriteLineAsync($"Response StatusCode : {stringResponseLog.StatusCode}");
                await writer.WriteLineAsync($"Total Time Taken for Api Request and Response Cycle {stringResponseLog.TotalTimeTaken}");

                await writer.WriteLineAsync(new String('-', 200)); // Separator for readability
            }
            catch (Exception ex)
            {
                await Task.Run(() => Console.WriteLine(ex));
            }
        }

    }
}
