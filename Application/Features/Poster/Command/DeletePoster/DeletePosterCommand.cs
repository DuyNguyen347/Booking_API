using Application.Interfaces.Poster;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Poster.Command.DeletePoster
{
    public class DeletePosterCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }

    }

    internal class DeletePosterCommandHandler : IRequestHandler<DeletePosterCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IPosterRepository _PosterRepository;

        public DeletePosterCommandHandler(
            IPosterRepository PosterRepository,
            IUnitOfWork<long> unitOfWork)
        {
            _PosterRepository = PosterRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(DeletePosterCommand request, CancellationToken cancellationToken)
        {
            var deletePoster = await _PosterRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (deletePoster == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            await _PosterRepository.DeleteAsync(deletePoster);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<long>.SuccessAsync("Delete success");
        }
    }
}
