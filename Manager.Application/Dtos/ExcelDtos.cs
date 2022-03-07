using System.Collections.Generic;

namespace Manager.Application.Dtos
{
    public class ExcelDto
    {
        public UserDto User { get; set; }

        public IList<ProjectExcelDto> Projects { get; set; }
    }
}