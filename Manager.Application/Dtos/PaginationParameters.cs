using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Dtos
{
    public class PaginationParameters
    {
        private string _orderBy = "Id";
        private string _keyword = string.Empty;

        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string Keyword { get => _keyword; set => _keyword = value ?? string.Empty; }
        public string OrderBy { get => _orderBy; set => _orderBy = ToPascalCase(value); }
        public string Direction { get; set; } = "ASC";

        public static string ToPascalCase(string value)
        {
            return CultureInfo.InvariantCulture.TextInfo
                .ToTitleCase(value);
        }
    }
}
