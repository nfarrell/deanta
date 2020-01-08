using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.ServiceLayer.Logger;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using Deanta.Cosmos.ServiceLayer.TodoSql;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deanta.Cosmos.Controllers
{
    public class SqlController : BaseDeantaController
    {
        public IActionResult Index (SqlSortFilterPageOptions options, [FromServices] ISqlListTodosService service)
        {
            var output = service.SortFilterPage(options).ToList();
            SetupTraceInfo();
            return View(new SqlTodoListCombinedDto(options, output));              
        }

        /// <summary>
        /// This provides the filter search dropdown content
        /// </summary>
        /// <param name="options"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFilterSearchContent    
            (SqlSortFilterPageOptions options, [FromServices]ITodoFilterDropdownService service)         
        {

            var traceIdent = HttpContext.TraceIdentifier; 
            return Json(                            
                new TraceIndentGeneric<IEnumerable<DropdownTuple>>(
                traceIdent,
                service.GetFilterDropDownValues(  options.FilterBy)));            
        }
        //-------------------------------------------------------

        public IActionResult ChangePubDate(Guid id, [FromServices]ICrudServices<DeantaSqlDbContext> service)
        {
            var dto = service.ReadSingle<ChangePubDateDto>(id);
            if (!service.IsValid)
            {
                service.CopyErrorsToModelState(ModelState, dto);
            }
            SetupTraceInfo();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePubDate(ChangePubDateDto dto, [FromServices]ICrudServicesAsync<DeantaSqlDbContext> service)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await service.UpdateAndSaveAsync(dto);
            SetupTraceInfo();
            if (service.IsValid)
                return View("TodoUpdated", service.Message);

            //Error state
            service.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        public IActionResult AddTodoComment(Guid id, [FromServices]ICrudServices<DeantaSqlDbContext> service)
        {
            var dto = service.ReadSingle<AddCommentDto>(id);
            if (!service.IsValid)
            {
                service.CopyErrorsToModelState(ModelState, dto);
            }
            SetupTraceInfo();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTodoComment(AddCommentDto dto, [FromServices]ICrudServices<DeantaSqlDbContext> service)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            service.UpdateAndSave(dto);
            SetupTraceInfo();
            if (service.IsValid)
                return View("TodoUpdated", service.Message);

            //Error state
            service.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }
    }
}
