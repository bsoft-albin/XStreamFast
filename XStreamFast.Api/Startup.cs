using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
    public class ConfigXStreamFastApplicationServer
    {
        private readonly WebApplication webApp;
        //private readonly IConfiguration config;
        //Using private readonly IApplicationBuilder webApp;    ==> (Legacy Approach in .NET 5 and Earlier)
        private readonly ConfigurationManager config;
        private readonly IServiceCollection services;
        private readonly IWebHostEnvironment iWebHostEnvironment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webAppBuilder"></param>
        public ConfigXStreamFastApplicationServer(WebApplicationBuilder webAppBuilder)
        {
            services = webAppBuilder.Services;
            config = webAppBuilder.Configuration;
            ConfigServices();
            webApp = webAppBuilder.Build();
            iWebHostEnvironment = webApp.Environment;
            ConfigServer();
        }

        private void ConfigServices()
        {
            services.AddControllers(x =>
            {
                // make every controller , need to accept https requests only!!
                x.Filters.Add(new RequireHttpsAttribute());
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

        private void ConfigServer()
        {
            // Initialize LogHelper with hosting environment
            ErrorLogger.Initialize(webApp.Services.GetRequiredService<IHostEnvironment>());

            //configuring the WebSockets Middleware
            //webApp.UseWebSockets();

            // Map WebSocket route using the custom extension method.
            //webApp.MapWebSocketRoute("/ws", webApp.Services.GetRequiredService<WebSocketHandler>());

            webApp.UseSwagger();

            IApiVersionDescriptionProvider provider = webApp.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            webApp.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            if (iWebHostEnvironment.IsDevelopment())
            {
                webApp.UseDeveloperExceptionPage();
            }

            if (!iWebHostEnvironment.IsDevelopment())
            {
                // adds a new middleware for using HSTS.
                webApp.UseHsts(); // HTTP Strict Transport Security!!!
            }

            // Configure the HTTP request pipeline.
            // Enforce HTTPS redirection middleware
            webApp.UseHttpsRedirection();

            //enforce additional security layer for Https requests only.
            webApp.UseMiddleware<HttpsRefererMiddleware>();

            webApp.UseAuthorization();

            webApp.UseCors(AppProps.Startup.CORS_POLICY);

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
