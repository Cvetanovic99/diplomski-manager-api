using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Manager.Application.Dtos;
using Manager.Application.Exceptions;
using Manager.Application.Interfaces;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Application.Specifications;
using Manager.Core.Entities;

namespace Manager.Application.Services
{
    public class PdfFileService : IPdfFileService
    {
        private readonly IAsyncRepository<PdfFile> _pdfFileRepository;
        private readonly IAsyncRepository<Project> _projectRepository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public PdfFileService(
            IAsyncRepository<PdfFile> pdfFileRepository, 
            IAsyncRepository<Project> projectRepository,
            IStorageService storageService,
            IMapper mapper)
        {
            this._pdfFileRepository = pdfFileRepository;
            this._projectRepository = projectRepository;
            this._storageService = storageService;
            this._mapper = mapper;

        }

        public async Task DeleteAsync(string filePath)
        {
            var pdfFile = await _pdfFileRepository.GetSingleBySpecAsync(new PdfFileByPathSpecification(filePath));
            if (pdfFile is null)
                throw new ApiException("Pdf fajl sa trazenom putanjom nije pronadjen.", 404);

            await _pdfFileRepository.DeleteAsync(pdfFile);
        }

        public async Task<PdfFileDto> CreateAsync(CreatePdfFileDto createPdfFileDto)
        {
            try
            {
                var project = await _projectRepository.GetSingleBySpecAsync(new ProjectByIdSpecification(createPdfFileDto.ProjectId));
                if (project is null)
                    throw new ApiException("Projekat nije pronadjen.", 404);

                if (project.PdfFiles.Count > 0)
                {
                    var oldPdfFile = project.PdfFiles.First();

                    await _storageService.DeleteAsync(oldPdfFile.Path);
                    project.PdfFiles.Remove(oldPdfFile);
                    await _pdfFileRepository.DeleteAsync(oldPdfFile);
                }

                var newPdfFile = new PdfFile() { Name = createPdfFileDto.Name, Path = createPdfFileDto.Path, Project = project};
                project.PdfFiles.Add(newPdfFile);
                await _projectRepository.UpdateAsync(project);

                return _mapper.Map<PdfFileDto>(newPdfFile);
            }
            catch (Exception e)
            {
                await _storageService.DeleteAsync(createPdfFileDto.Path);
                throw new ApiException($"Doslo je do greske prilikom upload-ovanja pdf fajla: {e.Message}", 500);
            }
        }

        public string DownloadPdfFile(string filePath)
        {
            return _storageService.GetFile(filePath);
        }
    }
}