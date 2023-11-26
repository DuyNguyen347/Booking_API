using Application.Interfaces.Merchant;
using Application.Parameters;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Application.Features.Merchant.Queries.GetAll
{
    public class GetAllMerchantQuery : RequestParameter, IRequest<PaginatedResult<GetAllMerchantResponse>>
    {
    }
    public class GetAllMerchantHandler : IRequestHandler<GetAllMerchantQuery, PaginatedResult<GetAllMerchantResponse>>
    {
        private readonly IMerchantRepository _MerchantRepository;
        public GetAllMerchantHandler(IMerchantRepository MerchantRepository)
        {
            _MerchantRepository = MerchantRepository;
        }
        public async Task<PaginatedResult<GetAllMerchantResponse>> Handle(GetAllMerchantQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = _MerchantRepository.Entities.AsEnumerable()
                        .Where(x => !x.IsDeleted
                                && (string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(x.MerchantName, request.Keyword)))
                        .AsQueryable()
                        .Select(x => new GetAllMerchantResponse
                        {
                            Id = x.Id,
                            MerchantName = x.MerchantName,
                            MerchantIpnUrl = x.MerchantIpnUrl,
                            MerchantReturnUrl = x.MerchantReturnUrl,
                            MerchantWebLink = x.MerchantWebLink,
                            CreatedOn = x.CreatedOn,
                            LastModifiedOn = x.LastModifiedOn
                        });
            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllMerchantResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllMerchantResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}
