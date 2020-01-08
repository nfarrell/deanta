
using System.ComponentModel.DataAnnotations;

namespace Deanta.Cosmos.ServiceLayer.TodosCommon
{
    public enum TodosFilterBy
    {
        [Display(Name = "All")]
        NoFilter = 0,
        [Display(Name = "By Day Created...")]
        ByCompleted
    }

    public enum OrderByOptions
    {
        [Display(Name = "Date Created ↑")] ByCreatedDate
    }
}