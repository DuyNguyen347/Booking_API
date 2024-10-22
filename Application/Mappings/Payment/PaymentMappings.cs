using Application.Dtos.Responses.Payment;
using Application.Features.Payment.Command;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings.Payment
{
    public class PaymentMappings : Profile
    {
        public PaymentMappings()
        {
            CreateMap<ProcessVnPayReturnCommand, VnPayReponse>().ReverseMap();
        }
    }
}
