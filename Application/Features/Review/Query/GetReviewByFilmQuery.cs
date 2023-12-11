using Application.Interfaces.Customer;
using Application.Interfaces.Film;
using Application.Interfaces.Repositories;
using Application.Interfaces.Review;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Review.Command.CancelVote;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Review.Query
{
    public class GetReviewByFilmQuery: IRequest<Result<GetReviewByFilmResponse>>
    {
        public long FilmId { get; set; }
    }
    public class GetReviewByFilmQueryHandler: IRequestHandler<GetReviewByFilmQuery, Result<GetReviewByFilmResponse>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;
        public GetReviewByFilmQueryHandler(
            ICustomerRepository customerRepository, 
            IFilmRepository filmRepository, 
            IReviewRepository reviewRepository, 
            ICurrentUserService currentUserService, 
            UserManager<AppUser> userManager)
        {
            _customerRepository = customerRepository;
            _filmRepository = filmRepository;
            _reviewRepository = reviewRepository;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }
        public async Task<Result<GetReviewByFilmResponse>> Handle(GetReviewByFilmQuery request, CancellationToken cancellationToken)
        {

            long CustomerId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            var existCustomer = await _customerRepository.FindAsync(x => x.Id == CustomerId && !x.IsDeleted);
            if (existCustomer == null) return await Result<GetReviewByFilmResponse>.FailAsync(StaticVariable.NOT_FOUND_CUSTOMER);

            var existFilm = await _filmRepository.FindAsync(x => x.Id == request.FilmId && !x.IsDeleted);
            if (existFilm == null) return await Result<GetReviewByFilmResponse>.FailAsync("NOT_FOUND_FILM");

            var existReview = await _reviewRepository.Entities
                .Where(x => x.CustomerId == CustomerId && x.FilmId == request.FilmId && !x.IsDeleted)
                .Select(x => new GetReviewByFilmResponse
                {
                    Score = x.Score
                }).FirstOrDefaultAsync();
            if (existReview == null) return await Result<GetReviewByFilmResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            return await Result<GetReviewByFilmResponse>.SuccessAsync(existReview);
        }
    }
}
