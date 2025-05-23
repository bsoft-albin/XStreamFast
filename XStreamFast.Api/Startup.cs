﻿using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using XStreamFast.Frameworks.CommonMeths;
using XStreamFast.Frameworks.CommonProps;

namespace XStreamFast.Api
{
    /// <summary>
    /// A Primary Class for Constructing XStreamFast Application Server.
    /// </summary>
    public class ConfigXStreamFastWebApplication
    {
        private readonly WebApplication webApp;
        //private readonly IConfiguration config;
        //Using private readonly IApplicationBuilder webApp;    ==> (Legacy Approach in .NET 5 and Earlier)
        private readonly ConfigurationManager config;
        private readonly IServiceCollection services;
        private readonly IServiceProvider provider;
        private readonly IWebHostEnvironment hostEnvironment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webAppBuilder"></param>
        public ConfigXStreamFastWebApplication(WebApplicationBuilder webAppBuilder)
        {
            services = webAppBuilder.Services;
            config = webAppBuilder.Configuration;
            ConfigServices();
            webApp = webAppBuilder.Build();
            provider = webApp.Services;
            hostEnvironment = webApp.Environment;
            ConfigLoggers();
        }

        /// <summary>
        /// Returns the Configured Web App.
        /// </summary>
        /// <returns></returns>
        public WebApplication GetConfiguredWebApp() { 
        
            return webApp;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConfigLoggers()
        {
            string getRootPath = provider.GetRequiredService<IWebHostEnvironment>().ContentRootPath;
            try
            {
                string getFullPath = HelperMeths.ReplaceLastSegment(getRootPath, "XStreamFast.FileServer");
                XStreamFastLoggers.Initialize(getFullPath);
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
                //send this exception as mail to admin of this app.
                ConfigXStreamWebApplication();
            }
        }

        private void ConfigServices()
        {
            services.AddControllers(x =>
            {
                // make every controller , need to accept https requests only!!
                x.Filters.Add(new RequireHttpsAttribute());

                // for Xml type Responses Supported by Controllers
                x.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                // need to add my custom class for CSV response formatters
            });

            // Enforce HTTPS redirection
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = 4000; // Change to the appropriate HTTPS port running in IIS web server.
            });

            //for API versioning
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            // Register the Swagger generator
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // add Memory cache Services.
            services.AddMemoryCache();

            // Add response compression services
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true; // Enable compression for HTTPS requests
                options.Providers.Add<GzipCompressionProvider>(); // Add Gzip compression provider
                //options.Providers.Add<BrotliCompressionProvider>(); // Add Brotli compression provider
            });

            // Configure compression options for Gzip
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest; // Set the compression level
            });

            // Load general configuration from appsettings.json for rate limitting
            services.Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"));

            // Add the AspNetCoreRateLimit services
            services.AddInMemoryRateLimiting();

            // Add services to the container.
            //service.AddSingleton<WebSocketHandler>();

            //Calling the custom Dependency Container Methods.
            DependencyContainer.AddServicesToDependencyContainer(services);

            services.AddEndpointsApiExplorer();

            //Helps to add XML Comments in the API Endpoints
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                 $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

                c.OperationFilter<SwaggerDefaultValues>();

                c.DocumentFilter<PathLowercaseDocumentFilter>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "OAuth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                 });
            });

            services.AddAuthentication(cfg => {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8
                        .GetBytes(config["JWTSetting:SecurityKey"] ?? "XZweufywefweRTYUIYRWEUY")
                    ),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Add HttpClient for third-party API calling
            services.AddHttpClient();

            // CORS policy configuration to allow any origin
            services.AddCors(options =>
            {
                options.AddPolicy(AppProps.Startup.CORS_POLICY, builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        /// <summary>
        /// A Method to Configure XStreamWebApplication.
        /// </summary>
        public void ConfigXStreamWebApplication()
        {
            //configuring the WebSockets Middleware
            //webApp.UseWebSockets();

            // Map WebSocket route using the custom extension method.
            //webApp.MapWebSocketRoute("/ws", webApp.Services.GetRequiredService<WebSocketHandler>());

            webApp.UseSwagger();

            IApiVersionDescriptionProvider versionProvider = provider.GetRequiredService<IApiVersionDescriptionProvider>();

            webApp.UseSwaggerUI(options =>
            {
                foreach (var description in versionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            if (hostEnvironment.IsDevelopment())
            {
                webApp.UseDeveloperExceptionPage();
            }

            if (!hostEnvironment.IsDevelopment())
            {
                // adds a new middleware for using HSTS.
                webApp.UseHsts(); // HTTP Strict Transport Security!!!
            }

            // Configure the HTTP request pipeline.
            // Enforce HTTPS redirection middleware
            webApp.UseHttpsRedirection();

            //enforce additional security layer for Https requests only.
            webApp.UseMiddleware<HttpsRefererMiddleware>();
            webApp.UseMiddleware<ApiLoggingMiddleware>();

            webApp.UseAuthorization();

            webApp.UseCors(AppProps.Startup.CORS_POLICY);

            webApp.Use(async (context, next) =>
            {
                #pragma warning disable
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("Permissions-Policy", "geolocation=(self), microphone=()");

                await next();
            });

            webApp.UseStaticFiles();// for serving static files in Server.

            // Telling the Server to Use response compression middleware.
            webApp.UseResponseCompression();

            //Telling the Server to Use response rate limiting middleware, for each IP address only 5->(setted in appsettings) requests are allowed.
            webApp.UseIpRateLimiting();
            //Server.UseClientRateLimiting();

            webApp.MapControllers();

            webApp.Run();
        }
    }
}
