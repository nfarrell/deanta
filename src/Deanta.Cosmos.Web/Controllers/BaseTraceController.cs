using Deanta.Cosmos.ServiceLayer.Logger;
using Microsoft.AspNetCore.Mvc;

namespace Deanta.Cosmos.Controllers
{
    /// <summary>
    /// To be plugged into Elastic APM
    /// </summary>
    public abstract class BaseDeantaController : Controller
    {
        protected void SetupTraceInfo()
        {
            ViewData["TraceIdent"] = HttpContext.TraceIdentifier;
            ViewData["NumLogs"] = HttpRequestLog.GetHttpRequestLog(HttpContext.TraceIdentifier).RequestLogs.Count;
        }
    }
}