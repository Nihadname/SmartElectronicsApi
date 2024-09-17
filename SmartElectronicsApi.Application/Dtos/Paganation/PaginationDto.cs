using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Paganation
{
    public class PaginationDto<T> : List<T>, IPaganationDto
    {
        public PaginationDto(List<T> items, int currentPage, int totalPage)
        {
            this.AddRange(items);
            CurrentPage = currentPage;
            TotalPage = totalPage;
        }

        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public bool HasNext => CurrentPage < TotalPage;
        public bool HasPrev => CurrentPage > 1;

        public static async Task<PaginationDto<T>> Create(IQueryable<T> query, int page, int Take)
        {
            var count = query.Count();
            var items = await query.Skip((page - 1) * Take).Take(Take).ToListAsync();
            var totalPages = (int)Math.Ceiling((decimal)count / Take);

            return new PaginationDto<T>(items, page, totalPages);
        }
    }
}
