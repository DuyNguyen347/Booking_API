using Application.Interfaces.Poster;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Poster.Command.AddPoster
{
    public class AddPosterCommand : IRequest<Result<AddPosterCommand>>
    {
        public long Id { get; set; }
        public string PathImage { get; set; } = default!;
        public string LinkUrl { get; set; } = default!;
    }

    internal class AddPosterCommandHandler : IRequestHandler<AddPosterCommand, Result<AddPosterCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IPosterRepository _PosterRepository;
        private readonly IUnitOfWork<long> _unitOfWork;


        public AddPosterCommandHandler(IMapper mapper, IPosterRepository PosterRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _PosterRepository = PosterRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AddPosterCommand>> Handle(AddPosterCommand request, CancellationToken cancellationToken)
        {
            var addPoster = _mapper.Map<Domain.Entities.Poster.Poster>(request);
            await _PosterRepository.AddAsync(addPoster);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = addPoster.Id;
            return await Result<AddPosterCommand>.SuccessAsync(request);
        }
    }
}
