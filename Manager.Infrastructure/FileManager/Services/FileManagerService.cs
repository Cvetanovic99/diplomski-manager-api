using ClosedXML.Excel;
using Manager.Application.Dtos;
using Manager.Application.Interfaces.ThirdPartyContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Infrastructure.FileManager
{
    public class FileManagerService : IFileManagerService
    {
        public string GetEmployeeReportAsExcelDocument(ExcelDto excelModel, IList<ProductDto> products)
        {
            string base64Document = string.Empty;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Izveštaj_sa_projekata");
                int currentRow = 1;
                int startProductColumn = 9;

                InsertHeader(worksheet, products, ref currentRow, startProductColumn);

                InsertProjectData(worksheet, excelModel, products, ref currentRow, startProductColumn);

                worksheet.Columns().AdjustToContents();

                base64Document = CreateBase64(workbook);
            }

            return base64Document;
        }

        private void InsertHeader(IXLWorksheet worksheet, IList<ProductDto> products, ref int currentRow, int startProductColumn)
        {
            worksheet.Cell(currentRow, 1).Value = "Projekat(klaster)";
            worksheet.Cell(currentRow, 2).Value = "Datum početka";
            worksheet.Cell(currentRow, 3).Value = "ID projekta";
            worksheet.Cell(currentRow, 4).Value = "Adresa korisnika";
            worksheet.Cell(currentRow, 5).Value = "Ime korisnika";
            worksheet.Cell(currentRow, 6).Value = "Kontakt";
            worksheet.Cell(currentRow, 7).Value = "Magacin";
            worksheet.Cell(currentRow, 8).Value = "Lokacija magacina";

            var productNames = products.Select(x => x.Name + " -" + x.Unit);
            worksheet.Cell(currentRow, startProductColumn).InsertData(productNames, true);

            worksheet.Cell(currentRow, startProductColumn + products.Count()).Value = "Status";
            worksheet.Cell(currentRow, startProductColumn + products.Count() + 1).Value = "Napomene montera";

            worksheet.Row(currentRow).Style.Font.Bold = true;
            worksheet.Row(currentRow).Style.Font.Italic = true;
            worksheet.Row(currentRow).Style.Font.FontSize = 13;

            currentRow++;
        }

        private void InsertProjectData(IXLWorksheet worksheet, ExcelDto excelModel, IList<ProductDto> products, ref int currentRow, int startProductColumn)
        {
            foreach (var project in excelModel.Projects)
            {
                worksheet.Cell(currentRow, 1).Value = project.Title;
                worksheet.Cell(currentRow, 2).Value = project.CreatedAt.ToString("dd-MMM-yyyy HH:mm");
                worksheet.Cell(currentRow, 3).Value = project.Id;
                worksheet.Cell(currentRow, 4).Value = project.Client.Address;
                worksheet.Cell(currentRow, 5).Value = project.Client.Name + "-" + project.Client.ClientId;
                worksheet.Cell(currentRow, 6).Value = project.Client.PhoneNumber;
                worksheet.Cell(currentRow, 7).Value = project.Warehouse.Name;
                worksheet.Cell(currentRow, 8).Value = project.Warehouse.City;
                worksheet.Cell(currentRow, 9 + products.Count()).Value = project.State;

                var descriptions = project.Tasks.Where(task => !string.IsNullOrEmpty(task.Description))
                                                    .Select(task => task.Description);

                var description = descriptions.Any() ? descriptions.Aggregate((a, b) => (a + " " + b)) : "";
                
                worksheet.Cell(currentRow, 10 + products.Count()).Value = description;

                #region products info for project

                int column = startProductColumn;

                foreach (var product in products)
                {
                    float quantity = project.Tasks.Where(task => task.ProductState is not null && task.ProductState.ProductId == product.Id)
                                            .Select(task => task.QuantityUsed)
                                            .Aggregate((float)0, (a, b) => a + b);

                    worksheet.Cell(currentRow, column).Value = quantity;
                    column++;
                }

                #endregion

                currentRow++;
            }

            worksheet.Rows(2, currentRow - 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
        }

        private string CreateBase64(XLWorkbook workbook)
        {
            string base64Document = string.Empty;

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                base64Document = Convert.ToBase64String(content);
            }

            return base64Document;
        }
    }
}
