using Application.Interfaces;
using Application.Parameters;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.Film;
using System.Linq.Dynamic.Core;
using Application.Interfaces.FilmImage;
using Application.Interfaces.Category;
using Application.Interfaces.CategoryFilm;

namespace Application.Features.Film.Queries.GetAll
{
    public class GetAllFilmQuery : RequestParameter, IRequest<PaginatedResult<GetAllFilmResponse>>
    {
        public bool Enable { get; set; }
    }

    internal class GetAllFilmHandler : IRequestHandler<GetAllFilmQuery, PaginatedResult<GetAllFilmResponse>>
    {
        private readonly IFilmRepository _filmRepository;
        private readonly IUploadService _uploadService;
        private readonly IFilmImageRepository _filmImageRepository;
        private readonly ICategoryFilmRepository _categoryFilmRepository;
        private readonly ICategoryRepository _categoryRepository;

        public GetAllFilmHandler(IFilmRepository filmRepository, IUploadService uploadService, IFilmImageRepository filmImageRepository, ICategoryFilmRepository categoryFilmRepository, ICategoryRepository categoryRepository)
        {
            _filmRepository = filmRepository;
            _uploadService = uploadService;
            _filmImageRepository = filmImageRepository;
            _categoryFilmRepository = categoryFilmRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<PaginatedResult<GetAllFilmResponse>> Handle(GetAllFilmQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = _filmRepository.Entities.AsEnumerable()
                        .Where(x => !x.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                                                || StringHelper.Contains(x.Name, request.Keyword) || x.Id.ToString().Contains(request.Keyword)))
                        .AsQueryable()
                        .Select(x => new GetAllFilmResponse
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Actor = x.Actor,
                            Director = x.Director,
                            Duration = x.Duration,
                            Description = x.Description,
                            Year = x.Year,
                            Country = x.Country,
                            LimitAge = x.LimitAge,
                            Trailer = x.Trailer,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate,
                            CreatedOn = x.CreatedOn,
                            LastModifiedOn = x.LastModifiedOn,
                            Image = _uploadService.GetFullUrl(_filmImageRepository.Entities.Where(_ => !_.IsDeleted && _.FilmId == x.Id).Select(y => y.NameFile).FirstOrDefault()),
                            Poster = _uploadService.GetFullUrl(x.Poster),
                            Category = GetCategory(x.Id)
                        });
            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllFilmResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllFilmResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
        public string GetCategory(long id)
        {
            var query = _categoryFilmRepository.Entities.Join(
                _categoryRepository.Entities,
                cateFilm => cateFilm.CategoryId,
                cate => cate.Id,
                (cateFilm, cate) => new
                {
                    Id = cateFilm.CategoryId,
                    Name = cate.Name,
                    FilmId = cateFilm.FilmId
                }
                )
                .Where(x => x.FilmId == id)
                .Select(result => result.Name).ToList();
            return string.Join(", ", query);
        }
    }
}
