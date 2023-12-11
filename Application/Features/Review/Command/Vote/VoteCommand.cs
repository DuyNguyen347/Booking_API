using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Application.Interfaces.Customer;
using Application.Interfaces.Film;
using Application.Interfaces.Repositories;
using Application.Interfaces.Review;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Review.Command.Vote
{
    public class VoteCommand : IRequest<Result<VoteCommand>>
    {
        public long Id { get; set; }
        public long FilmId { get; set; }
        public int Score { get; set; }
    }
    public class VoteCommandHandler : IRequestHandler<VoteCommand, Result<VoteCommand>>
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public VoteCommandHandler(
            IMapper mapper,
            ICustomerRepository customerRepository,
            IFilmRepository filmRepository,
            IReviewRepository reviewRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork<long> unitOfWork,
            UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
            _filmRepository = filmRepository;
            _reviewRepository = reviewRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<Result<VoteCommand>> Handle(VoteCommand request, CancellationToken cancellationToken)
        {
            long CustomerId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            /*Validate customer must have already bought the film ticket
            
            */ 

            var existCustomer = await _customerRepository.FindAsync(x => x.Id == CustomerId && !x.IsDeleted);
            if (existCustomer == null) return await Result<VoteCommand>.FailAsync(StaticVariable.NOT_FOUND_CUSTOMER);

            var existFilm = await _filmRepository.FindAsync(x => x.Id == request.FilmId && !x.IsDeleted);
            if (existFilm == null) return await Result<VoteCommand>.FailAsync("NOT_FOUND_FILM");

            var customerReview = _mapper.Map<Domain.Entities.Review.Review>(request);
            try
            {
                Validator.ValidateObject(customerReview, new ValidationContext(customerReview), true);
            }
            catch (ValidationException ex)
            {
                return await Result<VoteCommand>.FailAsync(ex.Message);
            }

            var existReview = await _reviewRepository.FindAsync(x => x.CustomerId == CustomerId && x.FilmId == request.FilmId && !x.IsDeleted);
            if (existReview == null)
            {
                await _reviewRepository.AddAsync(customerReview);
                await _unitOfWork.Commit(cancellationToken);
                request.Id = customerReview.Id;
            }
            else
            {
                existReview.Score = customerReview.Score;
                await _reviewRepository.UpdateAsync(existReview);
                await _unitOfWork.Commit(cancellationToken);
                request.Id = existCustomer.Id;
            }
            return await Result<VoteCommand>.SuccessAsync(request);
        }
    }
}
