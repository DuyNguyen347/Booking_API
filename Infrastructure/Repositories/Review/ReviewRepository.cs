using Application.Interfaces.Review;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Review
{
    public class ReviewRepository: RepositoryAsync<Domain.Entities.Review.Review, long>, IReviewRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ReviewRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public decimal? GetFilmReviewScore(long filmId)
        {
            var averageScore = _dbContext.Reviews
                .Where(_ => _.FilmId == filmId && !_.IsDeleted)
                .Select(_ => (decimal?)_.Score)
                .Average();
            return averageScore.HasValue ? Math.Round(averageScore.Value, 1) : null;
        }
        public int GetFilmNumberOfReviews(long filmId)
        {
            var count = _dbContext.Reviews.Count(_ => _.FilmId == filmId && !_.IsDeleted);
            return count;
        }
    }
}
