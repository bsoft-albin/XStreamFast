using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace XStreamFast.Api
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider = provider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = $".NET Core Api {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Contact = new OpenApiContact { Name = "Albin Anthony", Email = "albintony2002@gmail.com" },
                    License = new OpenApiLicense() { Name = "GitHub", Url = new Uri("https://github.com/") }
                });
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SwaggerDefaultValues : IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Deprecated = apiDescription.IsDeprecated();

            foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
            {
                var responseKey = responseType.StatusCode.ToString();
                var response = operation.Responses[responseKey];

                foreach (var contentType in response.Content.Keys)
                {
                    response.Content[contentType].Schema = context.SchemaGenerator.GenerateSchema(responseType.Type, context.SchemaRepository);
                }
            }
        }
    }
}
