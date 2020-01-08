
using System.Collections.Generic;
using System.Threading.Tasks;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using Deanta.Cosmos.ServiceLayer.TodosNoSql;
using Deanta.Cosmos.ServiceLayer.Logger;
using Microsoft.AspNetCore.Mvc;

namespace Deanta.Cosmos.Controllers
{
    public class NoSqlController : BaseDeantaController
    {
        public async Task<IActionResult> Index (NoSqlSortFilterPageOptions options, [FromServices] IListNoSqlTodosService service)
        {
            var output = await service.SortFilterPageAsync(options);
            SetupTraceInfo();
            return View(new NoSqlTodoListCombinedDto(options, output));              
        }
        /// <summary>
        /// This provides the filter search dropdown content
        /// </summary>
        /// <param name="options"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetFilterSearchContent    
            (NoSqlSortFilterPageOptions options, [FromServices]ITodoNoSqlFilterDropdownService service)         
        {

            var traceIdent = HttpContext.TraceIdentifier; 
            return Json(                            
                new TraceIndentGeneric<IEnumerable<DropdownTuple>>(
                traceIdent,
                await service.GetFilterDropDownValuesAsync(options.FilterBy)));            
        }

    }
}
