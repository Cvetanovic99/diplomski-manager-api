using System.Collections.Generic;

namespace Manager.Application.Dtos
{
    public class PaginationResponse<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public ICollection<T> Items { get; set; }
    }

    public class ProductPaginationResponse<T> : PaginationResponse<T>
    {
        public float Totalsum { get; set; }
    }
}