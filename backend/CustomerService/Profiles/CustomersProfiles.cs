using AutoMapper;
using CustomerService.Dtos;
using CustomerService.Models;

namespace CustomerService.Profiles;

public class CustomersProfiles : Profile
{
    public CustomersProfiles()
    {
        CreateMap<Customer, CustomerReadDto>();
        CreateMap<CustomerUpdateDto, Customer>();
        CreateMap<CustomerCreateDto, Customer>();

        CreateMap<Customer, CustomerPublishedDto>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
    }
}
