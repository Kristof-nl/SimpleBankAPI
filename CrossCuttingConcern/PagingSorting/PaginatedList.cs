using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossCuttingConcerns.PagingSorting
{
    public class PaginatedList<T>
    {
        public int CurrentPage { get; set; }
        public int From { get; set; }
        public List<T> Items { get; set; }
        public int PageSize { get; set; }
        public int To { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        
        public PaginatedList()
        {
        }
        public PaginatedList(List<T> items, int count, int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            PageSize = pageSize;
            From = ((currentPage - 1) * pageSize) + 1;
            To = (From + pageSize) - 1;
            Items = items;
        } 
        public bool HasPreviousPage
        {
            get
            {
                return (CurrentPage > 1);
            }
            set { }
        }

        public bool HasNextPage
        {
            get
            {
                return (CurrentPage < TotalPages);
            }
            set { }
        }
 
        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int currentPage, int pageSize, string sortOn, string sortDirection)
        {
            var count = await source.CountAsync();

            if (!string.IsNullOrEmpty(sortOn))
            {
                if (sortDirection.ToUpper() == "ASC")
                    source = source.OrderBy(sortOn);
                else
                    source = source.OrderByDescending(sortOn);
            }

            source = source.Skip((currentPage - 1) * pageSize)
                .Take(pageSize);

            var items = await source.ToListAsync();

            return new PaginatedList<T>(items, count, currentPage, pageSize);
        }
    }
}