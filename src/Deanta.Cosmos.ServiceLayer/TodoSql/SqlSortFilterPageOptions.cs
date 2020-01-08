﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Deanta.Cosmos.ServiceLayer.TodosCommon;
using Microsoft.EntityFrameworkCore;

namespace Deanta.Cosmos.ServiceLayer.TodoSql
{
    public class SqlSortFilterPageOptions
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
        /// <summary>
        /// This is set to the number of pages available based on the number of entries in the query
        /// </summary>
        public int NumPages { get; private set; }

        /// <summary>
        /// This holds the state of the key parts of the SortFilterPage parts 
        /// </summary>
        public string PrevCheckState { get; set; }
        public void SetupRestOfDto<T>(IQueryable<T> query)
        {
            SetupRestOfDtoGivenCount(query.Count());
        }

        public async Task SetupRestOfDtoAsync<T>(IQueryable<T> query)
        {
            SetupRestOfDtoGivenCount((await query.CountAsync()));
        }

        public void SetupRestOfDtoCosmosCount<T>(IQueryable<T> query)
        {
            var numFound = query.Select(_ => 1).AsEnumerable().ToList().Count;
            SetupRestOfDtoGivenCount(numFound);
        }

        //----------------------------------------
        //private methods

        private void SetupRestOfDtoGivenCount(int totalEntries)
        {
            NumPages = (int)Math.Ceiling(
                (double)totalEntries / PageSize);
            PageNum = Math.Min(
                Math.Max(1, PageNum), NumPages);

            var newCheckState = GenerateCheckState();
            if (PrevCheckState != newCheckState)
                PageNum = 1;

            PrevCheckState = newCheckState;
        }

        /// <summary>
        /// This returns a string containing the state of the SortFilterPage data
        /// that, if they change, should cause the PageNum to be set back to 0
        /// </summary>
        /// <returns></returns>
        private string GenerateCheckState()
        {
            return $"{(int) FilterBy},{FilterValue},{PageSize},{NumPages}";
        }
    }
}