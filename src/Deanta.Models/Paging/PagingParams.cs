namespace Deanta.Models.Paging
{
    /// <summary>
    /// Theoretically these should be default paging params - but I've removed the logic for simplicity.
    /// </summary>
    public class PagingParams
    {
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
