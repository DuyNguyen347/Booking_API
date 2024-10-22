using Application.Features.Review.Command.Vote;
using Application.Interfaces;
using Application.Interfaces.Customer;
using Application.Interfaces.Film;
using Application.Interfaces.Repositories;
using Application.Interfaces.Review;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Review.Command.CancelVote
{
    public class CancelVoteCommand: IRequest<Result<CancelVoteCommand>>
    {
        public long FilmId { get; set; }
    }
    public class CancelVoteCommandHandler: IRequestHandler<CancelVoteCommand, Result<CancelVoteCommand>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        public CancelVoteCommandHandler(
            IReviewRepository reviewRepository,
            ICustomerRepository customerRepository,
            IFilmRepository filmRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork<long> unitOfWork,
            UserManager<AppUser> userManager)
        {
            _reviewRepository = reviewRepository;
            _customerRepository = customerRepository;
            _filmRepository = filmRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<Result<CancelVoteCommand>> Handle(CancelVoteCommand request, CancellationToken cancellationToken)
        {
            long CustomerId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            var existCustomer = await _customerRepository.FindAsync(x => x.Id == CustomerId && !x.IsDeleted);
            if (existCustomer == null) return await Result<CancelVoteCommand>.FailAsync(StaticVariable.NOT_FOUND_CUSTOMER);

            var existFilm = await _filmRepository.FindAsync(x => x.Id == request.FilmId && !x.IsDeleted);
            if (existFilm == null) return await Result<CancelVoteCommand>.FailAsync("NOT_FOUND_FILM");

            var existReview = await _reviewRepository.FindAsync(x => x.CustomerId == CustomerId && x.FilmId == request.FilmId && !x.IsDeleted);
            if (existReview == null) return await Result<CancelVoteCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            await _reviewRepository.DeleteAsync(existReview);
            await _unitOfWork.Commit(cancellationToken);

            return await Result<CancelVoteCommand>.SuccessAsync(request);
        }
    }
}
