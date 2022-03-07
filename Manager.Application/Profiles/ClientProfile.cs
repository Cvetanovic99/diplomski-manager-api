using AutoMapper;
using Manager.Application.Dtos;
using Manager.Core.Entities;

namespace Manager.Application.Profiles
{
    public class ClientProfile: Profile
    {
        public ClientProfile()
        {
            CreateMap<CreateClientDto, Client>();
            CreateMap<UpdateClientDto, Client>();
            CreateMap<Client, ClientDto>();
        }
    }
}