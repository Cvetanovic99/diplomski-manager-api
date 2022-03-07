using System;
using System.IO;
using System.Threading.Tasks;
using Manager.Application.Dtos;
using Manager.Application.Exceptions;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Manager.Infrastructure.Storage.Services
{
    public class StorageService : IStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public StorageService(IWebHostEnvironment environment)
        {
            this._environment = environment;
        }

        public async Task<PdfFileDto> UploadAsync(IFormFile formFile, string nameWithoutExtension = null)
        {
            try
            {
                // var path = _environment.IsDevelopment() ?
                //                     Path.Combine(_environment.ContentRootPath, "pdffiles").ToLower()
                //                     :
                //                     Path.Combine("var/www/html/crm/manager-api/storage/pdffiles");

                var path = Path.Combine(_environment.ContentRootPath, "pdffiles").ToLower();
                
                var extension = Path.GetExtension(formFile.FileName);
                var fileName = $"{nameWithoutExtension ?? Guid.NewGuid().ToString()}{extension}";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fullFileLocation = Path.Combine(path, fileName).ToLower();

                using (var fileStream = new FileStream(fullFileLocation, FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream);
                }

                return new PdfFileDto { Path = $"pdffiles/{fileName}", Name = formFile.FileName };
            }
            catch (Exception)
            {
                throw new ApiException("Unable to upload file.", 500);
            }
        }

        public Task DeleteAsync(string filePath)
        {
            try
            {
                // var path = _environment.IsDevelopment() ?
                //     Path.Combine(_environment.ContentRootPath, filePath).ToLower()
                //     :
                //     Path.Combine("var/www/html/crm/manager-api/storage", filePath);

                var path = Path.Combine(_environment.ContentRootPath, filePath).ToLower();
                
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw new ApiException("Unable to delete file.", 500);
            }
        }

        public string GetFile(string filePath)
        {
            try
            {
                // var path = _environment.IsDevelopment() ?
                //     Path.Combine(_environment.ContentRootPath, filePath).ToLower()
                //     :
                //     Path.Combine("var/www/html/crm/manager-api/storage", filePath);

                var path = Path.Combine(_environment.ContentRootPath, filePath).ToLower();

                if (File.Exists(path))
                {
                    using (FileStream fs = File.Open(path, FileMode.Open))
                    {
                        MemoryStream ms = new MemoryStream();
                        fs.CopyTo(ms);

                        var byteArray = ms.ToArray();
                        return Convert.ToBase64String(byteArray);
                    }
                }
                else
                {
                    throw new ApiException("Fajl sa traženom putanjom ne postoji.", 404);
                }
            }
            catch (Exception e)
            {
                throw new ApiException($"Nije moguće skinuti fajl: {e.Message}", 500);
            }
        }
    }
}