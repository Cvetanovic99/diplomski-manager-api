using Manager.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Interfaces.ThirdPartyContracts
{
    public interface IFileManagerService
    {
        string GetEmployeeReportAsExcelDocument(ExcelDto excelModel, IList<ProductDto> allProducts);
    }
}
