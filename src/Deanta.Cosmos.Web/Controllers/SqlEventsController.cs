
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Deanta.Cosmos.DataLayerEvents.EfCode;
using Deanta.Cosmos.ServiceLayer.Logger;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using Deanta.Cosmos.ServiceLayer.TodoSql;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;
using Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents;
using Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents.Dtos;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deanta.Cosmos.Controllers
{
    public class SqlEventsController : BaseDeantaController
    {
        public IActionResult Index (SqlSortFilterPageOptions options, [FromServices]ISqlEventsListTodosService service)
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
            (SqlSortFilterPageOptions options, [FromServices]ITodoEventsFilterDropdownService service)         
        {

            var traceIdent = HttpContext.TraceIdentifier; 
            return Json(                            
                new TraceIndentGeneric<IEnumerable<DropdownTuple>>(
                traceIdent,
                service.GetFilterDropDownValues(  options.FilterBy)));            
        }

        //-------------------------------------------------------

        public IActionResult ChangePubDate(Guid id, [FromServices]ICrudServices<SqlEventsDbContext> service)
        {
            var dto = service.ReadSingle<ChangePubDateEventsDto>(id);
            if (!service.IsValid)
            {
                service.CopyErrorsToModelState(ModelState, dto);
            }
            SetupTraceInfo();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePubDate(ChangePubDateEventsDto dto, [FromServices]ICrudServicesAsync<SqlEventsDbContext> service)
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

        public IActionResult AddTodoComment(Guid id, [FromServices]ICrudServices<SqlEventsDbContext> service)
        {
            var dto = service.ReadSingle<AddCommentEventsDto>(id);
            if (!service.IsValid)
            {
                service.CopyErrorsToModelState(ModelState, dto);
            }
            SetupTraceInfo();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTodoComment(AddCommentEventsDto dto, [FromServices]ICrudServices<SqlEventsDbContext> service)
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
