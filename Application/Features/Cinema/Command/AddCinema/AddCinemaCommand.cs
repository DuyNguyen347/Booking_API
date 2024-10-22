using Application.Interfaces.Cinema;
using Application.Interfaces.CinemaImage;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Account;
using AutoMapper;
using Domain.Entities.CinemaImage;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Cinema.Command.AddCinema
{
    public class AddCinemaCommand : IRequest<Result<AddCinemaCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string City { get; set; } = default!;
        public string? Hotline { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? Address { get; set; }
        public List<string>? listImage { get; set; }
    }

    internal class AddCinemaCommandHandler : IRequestHandler<AddCinemaCommand, Result<AddCinemaCommand>>
    {
        private readonly IMapper _mapper;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly ICinemaImageRepository _cinemaImageRepository;
        private readonly IUnitOfWork<long> _unitOfWork;


        public AddCinemaCommandHandler(IMapper mapper, ICinemaRepository cinemaRepository, ICinemaImageRepository cinemaImageRepository, IUnitOfWork<long> unitOfWork, IAccountService accountService)
        {
            _mapper = mapper;
            _cinemaRepository = cinemaRepository;
            _unitOfWork = unitOfWork;
            _cinemaImageRepository = cinemaImageRepository;
        }

        public async Task<Result<AddCinemaCommand>> Handle(AddCinemaCommand request, CancellationToken cancellationToken)
        {
            var addCinema = _mapper.Map<Domain.Entities.Cinema.Cinema>(request);
            await _cinemaRepository.AddAsync(addCinema);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = addCinema.Id;
            if(request.listImage != null)
            {
                List<CinemaImage> newListCinemaImage = request.listImage.Select(x => new CinemaImage
                {
                    CinemaId = request.Id,
                    NameFile = x
                }).ToList();
                await _cinemaImageRepository.AddRangeAsync(newListCinemaImage);
                await _unitOfWork.Commit(cancellationToken);
            }
            return await Result<AddCinemaCommand>.SuccessAsync(request);
        }
    }
}
