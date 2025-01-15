using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Application.Models
{
    public class PaginatedList<T> : List<T>
    {

        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int CountData { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            CountData = count;

            AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return PageIndex > 1;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return PageIndex < TotalPages;
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex = 1, int pageSize = 10)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
        public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex = 1, int pageSize = 10)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        public static PaginatedList<T> Create(List<T> source, int pageIndex = 1, int pageSize = 10)
        {
            var count = source.Count;
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
    public class PaginatedOutput<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int CountData { get; private set; }
        public List<T> Data { get; set; }
        public PaginatedOutput(PaginatedList<T> values)
        {
            PageIndex = values.PageIndex;
            TotalPages = values.TotalPages;
            HasPreviousPage = values.HasPreviousPage;
            HasNextPage = values.HasNextPage;
            CountData = values.CountData;
            Data = values;
        }

    }
    public class PaginatedOutputServices<T>
    {
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; }
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage { get; set; }
        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage { get; set; }
        [JsonPropertyName("countData")]
        public int CountData { get; set; }
        [JsonPropertyName("data")]
        public List<T> Data { get; set; }

    }
}
