using Application.Interfaces;
using Application.Interfaces.Film;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Film
{
    public class FilmRepository : RepositoryAsync<Domain.Entities.Films.Film, long>, IFilmRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUploadService _uploadService;
        public FilmRepository(ApplicationDbContext dbContext, IUploadService uploadService) : base(dbContext)
        {
            _dbContext = dbContext;
            _uploadService = uploadService;
        }

        public string GetCategory(long filmId)
        {
            var categories = (from categoryFilm in _dbContext.CategoryFilms
                              where !categoryFilm.IsDeleted && categoryFilm.FilmId == filmId
                              join category in _dbContext.Categories
                              on categoryFilm.CategoryId equals category.Id
                              where !category.IsDeleted
                              select category.Name).ToList();
            return string.Join(", ", categories);
        }

        public string GetImage(long filmId)
        {
            var image = _uploadService.GetFullUrl(_dbContext.FilmImages.Where(_ => !_.IsDeleted && _.FilmId == filmId).Select(y => y.NameFile).FirstOrDefault());
            return image;
        }
    }
}
