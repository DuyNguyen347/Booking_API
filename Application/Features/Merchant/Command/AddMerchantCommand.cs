using Application.Exceptions;
using Application.Interfaces.Merchant;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Merchant.Command
{
    public class AddMerchantCommand : IRequest<Result<AddMerchantCommand>>
    {
        public string? MerchantName { get; set; } = string.Empty;
        public string? MerchantWebLink { get; set; } = string.Empty;
        public string? MerchantIpnUrl { get; set; } = string.Empty;
        public string? MerchantReturnUrl { get; set; } = string.Empty;

    }

    internal class AddMerchantCommandHandler : IRequestHandler<AddMerchantCommand, Result<AddMerchantCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IUnitOfWork<long> _unitOfWork;

        public AddMerchantCommandHandler(IMapper mapper, IMerchantRepository merchantRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _merchantRepository = merchantRepository; 
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AddMerchantCommand>> Handle(AddMerchantCommand request, CancellationToken cancellationToken)
        {
            var addMerchant = _mapper.Map<Domain.Entities.Merchant.Merchant>(request);
            addMerchant.IsActive = true;
            await _merchantRepository.AddAsync(addMerchant);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<AddMerchantCommand>.SuccessAsync(StaticVariable.SUCCESS);
        }
    }
}
