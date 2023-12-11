using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Application.Interfaces.FilmImage;
using Application.Interfaces.Film;
using Application.Interfaces.Category;
using Application.Interfaces.CategoryFilm;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Review;

namespace Application.Features.Film.Queries.GetById
{
    public class GetFilmByIdQuery : IRequest<Result<GetFilmByIdReponse>>
    {
        public long Id { get; set; }
    }
    internal class GetFilmByIdQueryHandler : IRequestHandler<GetFilmByIdQuery, Result<GetFilmByIdReponse>>
    {
        private readonly IFilmImageRepository _filmImageRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly IUploadService _uploadService;
        private readonly ICategoryFilmRepository _categoryFilmRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReviewRepository _reviewRepository;

        public GetFilmByIdQueryHandler(
            IFilmRepository filmRepository, 
            IFilmImageRepository filmImageRepository, 
            IUploadService uploadService, 
            ICategoryFilmRepository categoryFilmRepository, 
            ICategoryRepository categoryRepository,
            IReviewRepository reviewRepository)
        {
            _filmImageRepository = filmImageRepository;
            _filmRepository = filmRepository;
            _uploadService = uploadService;
            _categoryFilmRepository = categoryFilmRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<Result<GetFilmByIdReponse>> Handle(GetFilmByIdQuery request, CancellationToken cancellationToken)
        {
            var film =  _filmRepository.Entities.Where(x => x.Id == request.Id && !x.IsDeleted).Select(x => new GetFilmByIdReponse()
            {
                Id = x.Id,
                Name = x.Name,
                Actor = x.Actor,
                Director = x.Director,
                Producer = x.Producer,
                Duration = x.Duration,
                Description = x.Description,
                Year = x.Year,
                Country = x.Country,
                LimitAge = x.LimitAge,
                Trailer = x.Trailer,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                //Category = GetCategory(x.Id),
                CreatedOn = x.CreatedOn,
                Poster = _uploadService.GetFullUrl(x.Poster),
                //Image = _uploadService.GetFullUrl(_filmImageRepository.Entities.Where(_ => !_.IsDeleted && _.FilmId == x.Id).Select(y => y.NameFile).FirstOrDefault())
                Score = _reviewRepository.GetFilmReviewScore(x.Id)
            }).FirstOrDefault();
            
            if (film == null) return await Result<GetFilmByIdReponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            film.Category = GetCategory(film.Id);
            film.Image = GetListUrlImage(film.Id);
            return await Result<GetFilmByIdReponse>.SuccessAsync(film);
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

        public List<string> GetListUrlImage(long idFilm)
        {
            var listUrl = _filmImageRepository.Entities.Where(_ => !_.IsDeleted && _.FilmId == idFilm).Select(y => y.NameFile).ToList();
            List<string> listFullUrl = new List<string>();
            foreach(string? i in listUrl)
            {
                listFullUrl.Add(_uploadService.GetFullUrl(i));
            }
            return listFullUrl;
        }
    }
}
