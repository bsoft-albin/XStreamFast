using Microsoft.AspNetCore.Mvc;
using System.Collections;
using XStreamFast.Frameworks.CommonMeths;

namespace XStreamFast.Api.Controllers
{
    /// <summary>
    /// A Base Api Controller for XStreamFast
    /// </summary>
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        protected ContentResult HtmlResponseFormatter(string htmlContent) {

            return Content(htmlContent, "text/html");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textContent"></param>
        /// <returns></returns>
        protected ContentResult TextResponseFormatter(string textContent)
        {

            return Content(textContent, "text/plain");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlContent"></param>
        /// <returns></returns>
        protected ContentResult XmlResponseFormatter(object xmlContent)
        {

            return Content(HelperMeths.SerializeToXml(xmlContent), "application/xml");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        protected ContentResult JsonResponseFormatter(object jsonContent)
        {

            return Content(HelperMeths.SerializeToJson(jsonContent), "application/json");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="downloadFileName"></param>
        /// <returns></returns>
        protected FileContentResult BinaryResponseFormatter(string filePath, string downloadFileName)
        {
            byte[] fileContents = System.IO.File.ReadAllBytes(filePath);
            return File(fileContents, "application/octet-stream", downloadFileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="downloadFileName"></param>
        /// <returns></returns>
        protected CsvResult CsvResponseFormatter(IEnumerable data, string downloadFileName)
        {

            return new CsvResult(data, downloadFileName);
        }

    }
}
