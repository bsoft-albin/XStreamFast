using XStreamFast.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigXStreamFastWebApplication app = new(builder);

app.ConfigXStreamWebApplication();

Console.WriteLine("Web Application started");

