using XStreamFast.Api;
using XStreamFast.Frameworks.CommonMeths;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigXStreamFastApplicationServer appServer = new(builder);

WebApplication web = appServer.GetConfiguredWebApp();

// Initialize LogHelper with hosting environment
ErrorLogger.Initialize(HelperMeths.ReplaceLastSegment(web.Services.GetRequiredService<IWebHostEnvironment>().ContentRootPath, "XStreamFast.FileServer"));
