using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Text;

namespace XStreamFast.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class CsvResult : IActionResult
    {
        private readonly IEnumerable _data;
        private readonly string _fileName;

        public CsvResult(IEnumerable data, string fileName = "file.csv")
        {
            _data = data;
            _fileName = fileName;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "text/csv";
            response.Headers.Add("Content-Disposition", $"attachment; filename={_fileName}");

            StringBuilder buffer = new StringBuilder();
            Type itemType = _data.GetType().GenericTypeArguments[0];
            var properties = itemType.GetProperties();

            // Create CSV header
            buffer.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Add data rows
            foreach (var item in _data)
            {
                var values = properties.Select(p => p.GetValue(item)?.ToString() ?? string.Empty);
                buffer.AppendLine(string.Join(",", values));
            }

            await response.WriteAsync(buffer.ToString());
        }
    }
}
