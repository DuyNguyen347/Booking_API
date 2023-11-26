using Application.Features.Merchant.Command;
using AutoMapper;

namespace Application.Mappings.Merchant
{
    public class MerchantMappings : Profile
    {
        public MerchantMappings()
        {
            CreateMap<Domain.Entities.Merchant.Merchant, AddMerchantCommand>().ReverseMap();
            CreateMap<Domain.Entities.Merchant.Merchant, EditMerchantCommand>().ReverseMap();
        }
    }
}
