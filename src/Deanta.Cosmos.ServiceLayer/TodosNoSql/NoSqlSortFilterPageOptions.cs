﻿
using Deanta.Cosmos.ServiceLayer.TodosCommon;

namespace Deanta.Cosmos.ServiceLayer.TodosNoSql
{
    public class NoSqlSortFilterPageOptions
    {
        public const int DefaultPageSize = 10;   //default page size is 10

        /// <summary>
        /// This holds the possible page sizes
        /// </summary>
        public int[] PageSizes = new[] {5, DefaultPageSize, 20, 50, 100, 500, 1000};

        public OrderByOptions OrderByOptions { get; set; }

        public TodosFilterBy FilterBy { get; set; }

        public string FilterValue { get; set; }

        //-----------------------------------------
        //Paging parts, which require the use of the method

        public int PageNum { get; set; } = 1;

        public int PageSize { get; set; } = DefaultPageSize;

        public bool PrevPageValid { get; private set; }
        public bool NextPageValid { get; private set; }

        /// <summary>
        /// This holds the state of the key parts of the SortFilterPage parts 
        /// </summary>
        public string PrevCheckState { get; set; }
        public void SetupRestOfDto(int numEntriesRead)
        {
            SetupRestOfDtoGivenCount(numEntriesRead);
        }

        //----------------------------------------
        //private methods

        private void SetupRestOfDtoGivenCount(int numEntriesRead)
        {
            var newCheckState = GenerateCheckState();
            if (PrevCheckState != newCheckState)
                PageNum = 1;

            NextPageValid = numEntriesRead == PageSize;
            PrevPageValid = PageNum > 1;
            PrevCheckState = newCheckState;
        }

        /// <summary>
        /// This returns a string containing the state of the SortFilterPage data
        /// that, if they change, should cause the PageNum to be set back to 0
        /// </summary>
        /// <returns></returns>
        private string GenerateCheckState()
        {
            return $"{(int) FilterBy},{FilterValue},{PageSize}";
        }
    }
}