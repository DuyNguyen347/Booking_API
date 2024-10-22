using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Review
{
    public interface IReviewRepository: IRepositoryAsync<Domain.Entities.Review.Review, long>
    {
        decimal? GetFilmReviewScore(long filmId);
        int GetFilmNumberOfReviews(long filmId);
    }
}
