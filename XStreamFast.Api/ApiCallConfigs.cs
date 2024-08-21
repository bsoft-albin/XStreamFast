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
            // Log request details
            var requestLog = await LogRequest(context);

            // Intercept the response body stream
            var originalResponseBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            // Proceed to the next middleware
            await _next(context);

            // Log response details
            await LogResponse(context, requestLog);

            // Copy the intercepted response body back to the original response body stream
            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        }

        private async Task<RequestLog> LogRequest(HttpContext context)
        {
            HttpRequest request = context.Request;

            var requestLog = new RequestLog
            {
                HttpMethod = request.Method,
                Endpoint = $"{request.Method} {request.Path}",
                Headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Body = await ReadRequestBodyAsync(request)
            };

            string strHttpMethod = requestLog.HttpMethod;
            string strHeader = JsonConvert.SerializeObject(requestLog.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
            string strBody = JsonConvert.SerializeObject(requestLog.Body);
            string strEndpoint = $"{request.Method} {request.Path}";

            // Log HTTP method, endpoint, headers
            await XStreamFastLoggers.WriteApiRequestLog(strHttpMethod, strEndpoint, strHeader, strBody, "XStreamFast: {@RequestLog}");

            return requestLog;
        }

        private async Task LogResponse(HttpContext context, RequestLog requestLog)
        {
            HttpResponse response = context.Response;

            // Log HTTP method, endpoint, headers, status code, and response body
            var responseLog = new ResponseLog
            {
                HttpMethod = requestLog.HttpMethod,
                Endpoint = requestLog.Endpoint,
                Headers = response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                StatusCode = response.StatusCode,
                Body = await ReadResponseBodyAsync(response)
            };

            string strHttpMethod = responseLog.HttpMethod;
            string strHeader = JsonConvert.SerializeObject(responseLog.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
            string strBody = JsonConvert.SerializeObject(responseLog.Body);
            string strEndpoint = requestLog.Endpoint;

            await XStreamFastLoggers.WriteApiResponseLog(strHttpMethod, strEndpoint, strHeader, strBody, responseLog.StatusCode, "XStreamFast: {@ResponseLog}");
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
            response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return !string.IsNullOrEmpty(body) ? JsonConvert.DeserializeObject(body) : null;
        }
    }

}
