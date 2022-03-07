using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Manager.Application.Dtos;
using Manager.Application.Exceptions;
using Manager.Application.Interfaces;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Application.Specifications;
using Manager.Core.Entities;
using System.Linq.Expressions;
using System;

namespace Manager.Application.Services
{
    public class ClientService: IClientService
    {
        private readonly IAsyncRepository<Client> _clientRepository;
        private readonly IMapper _mapper;

        public ClientService(
            IAsyncRepository<Client> clientRepository,
            IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }
        
        public async Task<PaginationResponse<ClientDto>> GetClientsAsync(PaginationParameters paginationParameters)
        {
            Expression<Func<Client, bool>> criteria = c => c.Name.Contains(paginationParameters.Keyword) || c.ClientId.Contains(paginationParameters.Keyword);

            var clients = await _clientRepository.GetBySpecAsync(
                new ClientSpecification(
                    criteria: criteria,
                    start: paginationParameters.PageIndex * paginationParameters.PageSize,
                    take: paginationParameters.PageSize,
                    orderBy: paginationParameters.OrderBy,
                    direction: paginationParameters.Direction));

            var count = await _clientRepository.GetCountBySpecAsync(
                new ClientSpecification(criteria));

            return new PaginationResponse<ClientDto>()
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<ClientDto>>(clients),
                Total = count
            };
        }

        public async Task<IList<ClientDto>> GetAllClientsAsync()
        {
            var clients = await _clientRepository.GetAsync();

            return _mapper.Map<IList<ClientDto>>(clients);
        }

        public async Task<ClientDto> GetClientByIdAsync(int clientId)
        {
            var client = await _clientRepository.GetByIdAsync(clientId);

            if (client is null)
            {
                throw new ApiException("Klijent nije pronadjen", 404);
            }

            return _mapper.Map<ClientDto>(client);
        }

        public async Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto)
        {
            var client = _mapper.Map<Client>(createClientDto);

            await _clientRepository.AddAsync(client);
            
            return _mapper.Map<ClientDto>(client);
        }

        public async Task<ClientDto> UpdateClientAsync(UpdateClientDto updateClientDto)
        {
            var client = await _clientRepository.GetByIdAsync(updateClientDto.Id);

            if (client is null)
            {
                throw new ApiException("Klijent nije pronadjen", 404);
            }

            _mapper.Map(updateClientDto, client);

            await _clientRepository.UpdateAsync(client);
            
            return _mapper.Map<ClientDto>(client);
        }

        public async Task DeleteClientAsync(int clientId)
        {
            var client = await _clientRepository.GetByIdAsync(clientId);

            if (client is null)
            {
                throw new ApiException("Klijent nije pronadjen", 404);
            }

            await _clientRepository.DeleteAsync(client);
        }
    }
}