﻿using Application.Interfaces.Poster;
using Application.Parameters;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Application.Features.Poster.Queries.GetAll
{
    public class GetAllPosterQuery : RequestParameter, IRequest<PaginatedResult<GetAllPosterReponse>>
    {
    }
    public class GetAllPosterHandler : IRequestHandler<GetAllPosterQuery, PaginatedResult<GetAllPosterReponse>>
    {
        private readonly IPosterRepository _PosterRepository;
        public GetAllPosterHandler(IPosterRepository PosterRepository)
        {
            _PosterRepository = PosterRepository;
        }
        public async Task<PaginatedResult<GetAllPosterReponse>> Handle(GetAllPosterQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = _PosterRepository.Entities.AsEnumerable()
                        .Where(x => !x.IsDeleted)
                        .AsQueryable()
                        .Select(x => new GetAllPosterReponse
                        {
                            Id = x.Id,
                            CreatedOn = x.CreatedOn,
                            LastModifiedOn = x.LastModifiedOn
                        });
            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllPosterReponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllPosterReponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}
