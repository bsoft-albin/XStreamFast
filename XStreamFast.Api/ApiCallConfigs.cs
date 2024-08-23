using Newtonsoft.Json;
using XStreamFast.Frameworks.CommonMeths;
using XStreamFast.Models;

namespace XStreamFast.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public ApiLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Record the start time
            DateTime startTime = DateTime.UtcNow;

            // Log request details
            var requestLog = await LogRequest(context);

            // Intercept the response body stream
            var originalResponseBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            try
            {
                // Proceed to the next middleware
                await _next(context);
            }
            catch (Exception x)
            {
                await XStreamFastLoggers.WriteExceptionLog(x, $"Exception Occured for procsssing this Api [@Request Call] Endpoint => : {requestLog.Endpoint}");
            }
            finally
            {
                // Record the end time
                DateTime endTime = DateTime.UtcNow;

                // Calculate the processing time
                TimeSpan processingTime = endTime - startTime;

                // Log response details
                await LogResponse(context, requestLog, processingTime);

                // Copy the intercepted response body back to the original response body stream
                await responseBodyStream.CopyToAsync(originalResponseBodyStream);
            }

        }

        private async Task<RequestLog> LogRequest(HttpContext context)
        {

            HttpRequest request = context.Request;

            StringRequestLog logRequest = new();   

            // Check for the forwarded header
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
#pragma warning disable
                logRequest.ClientIpAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }
            else
            {
                // Fall back to the remote IP address
                logRequest.ClientIpAddress = context.Connection.RemoteIpAddress.ToString();
            }

            // If you want to convert IPv6 loopback to IPv4 loopback for local testing
            if (logRequest.ClientIpAddress == "::1")
            {
                logRequest.ClientIpAddress = "127.0.0.1";
            }

            logRequest.CorrelationId = context.Request.Headers["Correlation-ID"].FirstOrDefault();

            if (string.IsNullOrEmpty(logRequest.CorrelationId))
            {
                logRequest.CorrelationId = Guid.NewGuid().ToString();
            }

            logRequest.UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(); // which device and application makes this request browser, mobile etc..

            logRequest.UserIdentity = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Anonymous";

            logRequest.QueryParameters = "No Query Parameters";

            if (request.Query.Count > 0)
            {
                int QueryStringCount = 0;

                foreach (var item in request.Query)
                {
                    QueryStringCount++;

                    if (QueryStringCount == 1) {
                        logRequest.QueryParameters = "";
                        logRequest.QueryParameters += "{";
                    }

                    logRequest.QueryParameters += $"Key[{item.Key}]" + ":" + $"Value[{item.Value}]";

                    if (QueryStringCount == request.Query.Count)
                    {
                        logRequest.QueryParameters += "}";
                    }

                    if (!(QueryStringCount == request.Query.Count))
                    {
                        logRequest.QueryParameters += "&";
                    }
                }
            }

            var requestLog = new RequestLog
            {
                HttpMethod = request.Method,
                Endpoint = $"{request.Method} {request.Path}",
                Headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Body = await ReadRequestBodyAsync(request)
            };

            logRequest.HttpMethod = requestLog.HttpMethod;
            logRequest.Headers = JsonConvert.SerializeObject(requestLog.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
            logRequest.Body = JsonConvert.SerializeObject(requestLog.Body);
            logRequest.Endpoint = $"{request.Method} {request.Path}";

            logRequest.Message = "XStreamFast: {@RequestLog}";

            // Log HTTP method, endpoint, headers
            await XStreamFastLoggers.WriteApiRequestLog(logRequest);

            return requestLog;
        }

        private async Task LogResponse(HttpContext context, RequestLog requestLog, TimeSpan totalTimeTaken)
        {

            HttpResponse response = context.Response;

            StringResponseLog stringResponseLog = new();

            // Log HTTP method, endpoint, headers, status code, and response body
            var responseLog = new ResponseLog
            {
                HttpMethod = requestLog.HttpMethod,
                Endpoint = requestLog.Endpoint,
                Headers = response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                StatusCode = response.StatusCode,
                Body = await ReadResponseBodyAsync(response)
            };

            stringResponseLog.TotalTimeTaken = "[Hrs:Min:Sec:MSec] : " + "[" + totalTimeTaken.Hours + ":" + totalTimeTaken.Minutes + ":" + totalTimeTaken.Seconds + ":" + totalTimeTaken.Milliseconds + "]";

            stringResponseLog.HttpMethod = responseLog.HttpMethod;
            stringResponseLog.Headers = JsonConvert.SerializeObject(responseLog.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
            stringResponseLog.Body = JsonConvert.SerializeObject(responseLog.Body);
            stringResponseLog.Endpoint = requestLog.Endpoint;

            stringResponseLog.StatusCode = responseLog.StatusCode;
            stringResponseLog.Message = "XStreamFast: {@ResponseLog}";

            await XStreamFastLoggers.WriteApiResponseLog(stringResponseLog);
        }

        private async Task<object> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            return !string.IsNullOrEmpty(body) ? JsonConvert.DeserializeObject(body) : null;
        }

        private async Task<object> ReadResponseBodyAsync(HttpResponse response)
        {
            var body = "";
            try
            {
                // Rewind the response body stream to the beginning
                response.Body.Seek(0, SeekOrigin.Begin);

                // Read the response body as a string
                body = await new StreamReader(response.Body).ReadToEndAsync();

                // Rewind the response body stream again for further processing
                response.Body.Seek(0, SeekOrigin.Begin);

                // Check if the content type is JSON
                if (response.ContentType != null && response.ContentType.Contains("application/json"))
                {
                    return !string.IsNullOrEmpty(body) ? JsonConvert.DeserializeObject(body) : null;
                }
            }
            catch (Exception x)
            {

                await XStreamFastLoggers.WriteExceptionLog(x, $"Exception Occured for procsssing this Api [@Response Call] StatucCode => : {response.StatusCode}");
            }

            // If not JSON, return the raw body or handle as needed
            return body;
        }
    }

}
