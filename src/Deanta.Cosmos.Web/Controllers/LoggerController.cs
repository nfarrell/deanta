
using Deanta.Cosmos.ServiceLayer.Logger;
using Microsoft.AspNetCore.Mvc;

namespace Deanta.Cosmos.Controllers
{
    /// <summary>
    /// Use Elastic APM!
    /// </summary>
    public class LoggerController : Controller
    {
        [HttpGet]
        public JsonResult GetLog(string traceIdentifier)
        {
            return Json(HttpRequestLog.GetHttpRequestLog(traceIdentifier));
        }
    }
}
