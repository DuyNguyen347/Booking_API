using Application.Interfaces.Cinema;
using Application.Parameters;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Application.Features.Cinema.Queries.GetAll
{
    public class GetAllCinemaQuery : RequestParameter, IRequest<PaginatedResult<GetAllCinemaResponse>>
    {
        public string? City { get; set; }
    }
    public class GetAllCinemaHandler : IRequestHandler<GetAllCinemaQuery, PaginatedResult<GetAllCinemaResponse>>
    {
        private readonly ICinemaRepository _cinemaRepository;
        public GetAllCinemaHandler(ICinemaRepository cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }
        public async Task<PaginatedResult<GetAllCinemaResponse>> Handle(GetAllCinemaQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            if (request.City != null)
                request.City = request.City.Trim();

            Console.WriteLine("thanh pho " + request.City);

            var query = _cinemaRepository.Entities.AsEnumerable()
                        .Where(x => !x.IsDeleted 
                                && (string.IsNullOrEmpty(request.City) 
                                || x.City == request.City)
                                && (string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(x.Name, request.Keyword)))
                        .AsQueryable()
                        .Select(x => new GetAllCinemaResponse
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            City = x.City,
                            CreatedOn = x.CreatedOn,
                            LastModifiedOn = x.LastModifiedOn
                        });
            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllCinemaResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllCinemaResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}
