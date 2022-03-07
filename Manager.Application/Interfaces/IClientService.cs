using System.Collections.Generic;
using System.Threading.Tasks;
using Manager.Application.Dtos;

namespace Manager.Application.Interfaces
{
    public interface IClientService
    {
        Task<PaginationResponse<ClientDto>> GetClientsAsync(PaginationParameters paginationParameters);
        Task<IList<ClientDto>> GetAllClientsAsync();
        Task<ClientDto> GetClientByIdAsync(int clientId);
        Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto);
        Task<ClientDto> UpdateClientAsync(UpdateClientDto updateClient);
        Task DeleteClientAsync(int clientId);
    }
}