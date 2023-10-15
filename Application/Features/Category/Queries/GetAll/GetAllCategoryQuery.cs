using Application.Features.Cusomter.Queries.GetAll;
using Application.Interfaces.Category;
using Application.Parameters;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Application.Features.Category.Queries.GetAll
{
    public class GetAllCategoryQuery : RequestParameter, IRequest<PaginatedResult<GetAllCategoryResponse>>
    {
    }
    public class GetAllCategoryHandler : IRequestHandler<GetAllCategoryQuery, PaginatedResult<GetAllCategoryResponse>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetAllCategoryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<PaginatedResult<GetAllCategoryResponse>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = _categoryRepository.Entities.AsEnumerable()
                        .Where(x => !x.IsDeleted
                                && (string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(x.Name, request.Keyword)))
                        .AsQueryable()
                        .Select(x => new GetAllCategoryResponse
                        {
                            Id = x.Id,
                            Name = x.Name,
                            CreatedOn = x.CreatedOn,
                            LastModifiedOn = x.LastModifiedOn
                        });
            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllCategoryResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllCategoryResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}
