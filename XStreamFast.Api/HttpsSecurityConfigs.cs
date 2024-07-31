namespace XStreamFast.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpsRefererMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public HttpsRefererMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var referer = context.Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(referer) && Uri.TryCreate(referer, UriKind.Absolute, out var refererUri))
            {
                if (refererUri.Scheme != Uri.UriSchemeHttps)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("HTTPS Referer required.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
