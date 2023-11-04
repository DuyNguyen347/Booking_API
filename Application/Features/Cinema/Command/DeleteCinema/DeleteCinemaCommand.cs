using Application.Interfaces.Cinema;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Cinema.Command.DeleteCinema
{
    public class DeleteCinemaCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }

    }

    internal class DeleteCinemaCommandHandler : IRequestHandler<DeleteCinemaCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICinemaRepository _cinemaRepository;

        public DeleteCinemaCommandHandler(
            ICinemaRepository cinemaRepository,
            IUnitOfWork<long> unitOfWork)
        {
            _cinemaRepository = cinemaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(DeleteCinemaCommand request, CancellationToken cancellationToken)
        {
            var deleteCinema = await _cinemaRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (deleteCinema == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            await _cinemaRepository.DeleteAsync(deleteCinema);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<long>.SuccessAsync("Delete success");
        }
    }
}
