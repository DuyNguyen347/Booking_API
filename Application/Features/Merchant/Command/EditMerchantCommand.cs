using Application.Interfaces.Merchant;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Merchant.Command
{
    public class EditMerchantCommand : IRequest<Result<EditMerchantCommand>>
    {
        public string? MerchantName { get; set; } = string.Empty;
        public string? MerchantIpnUrl { get; set; } = string.Empty;
        public string? MerchantReturnUrl { get; set; } = string.Empty;

    }

    internal class EditMerchantCommandHandler : IRequestHandler<EditMerchantCommand, Result<EditMerchantCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IUnitOfWork<long> _unitOfWork;

        public EditMerchantCommandHandler(IMapper mapper, IMerchantRepository merchantRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _merchantRepository = merchantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<EditMerchantCommand>> Handle(EditMerchantCommand request, CancellationToken cancellationToken)
        {
            var EditMerchant = await _merchantRepository.Entities.FirstOrDefaultAsync();
            if(EditMerchant == null)
            {
                return await Result<EditMerchantCommand>.FailAsync(request, StaticVariable.NOT_FOUND_MSG);
            }
            _mapper.Map(request, EditMerchant);
            await _merchantRepository.UpdateAsync(EditMerchant);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<EditMerchantCommand>.SuccessAsync(StaticVariable.SUCCESS);
        }
    }
}
