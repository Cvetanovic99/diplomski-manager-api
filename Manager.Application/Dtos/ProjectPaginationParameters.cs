using System.Globalization;

namespace Manager.Application.Dtos
{
    public class ProjectPaginationParameters : PaginationParameters
    {
        public string State { get; set; }
    }
}