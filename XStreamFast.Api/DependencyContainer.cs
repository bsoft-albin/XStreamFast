using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;
using Polly.CircuitBreaker;
using Polly;
using Swashbuckle.AspNetCore.SwaggerGen;
using XStreamFast.DbEngine;
using XStreamFast.Repositories.Interfaces;
using XStreamFast.Repositories.Implementations;
using XStreamFast.Services.Implementations;
using XStreamFast.Services.Interfaces;

namespace XStreamFast.Api
{
    /// <summary>
    /// 
    /// </summary>
    public static class DependencyContainer
    {
        /// <summary>
        /// A Common method for calling Other Dependency Classes.
        /// </summary>
        /// <param name="services">A Configured Services.</param>
        public static void AddServicesToDependencyContainer(IServiceCollection services)
        {
            AddDbEngineLayerServices(services);
            AddRepositoryLayerServices(services);
            AddServiceLayerServices(services);
            AddOtherServices(services);
        }

        //DbEngine Layer Registering
        private static void AddDbEngineLayerServices(IServiceCollection services)
        {
            services.AddScoped<IPostgresMapper, PostgresMapper>();
            //services.AddTransient<IMysqlDapper, MysqlHandler>();
            services.AddTransient<ISqlServerMapper, SqlServerMapper>();
        }

        //Repository Layer Registering
        private static void AddRepositoryLayerServices(IServiceCollection services)
        {
            services.AddScoped<IPostgresRepo, PostgresRepo>();
        }

        //Services Layer Registering
        private static void AddServiceLayerServices(IServiceCollection services)
        {
            services.AddScoped<IPostgresServices, PostgresServices>();
        }

        //To Register Other Services
        private static void AddOtherServices(IServiceCollection services)
        {
            // for API versioning
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            // for Caching Feature
            // Inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // for rate limiting feature
            // Configure the clients policies (in-memory)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // for circiut breaking policy.
            // Define the circuit breaker policy
            AsyncCircuitBreakerPolicy circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (ex, breakDelay) => {
                        Console.WriteLine($"Circuit broken due to {ex.Message}. Breaking for {breakDelay.TotalSeconds} seconds.");
                    },
                    onReset: () => {
                        Console.WriteLine("Circuit reset.");
                    },
                    onHalfOpen: () => {
                        Console.WriteLine("Circuit is half-open. Testing for recovery.");
                 });

            services.AddSingleton(circuitBreakerPolicy);

            services.AddTransient<JWTTokenService>();

        }
    }
}
