using Application.Interfaces.Film;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Film.Command.ChangeFilmEnable
{
    public class ChangeFilmEnableCommand: IRequest<Result<long>>
    {
        public long FilmId { get; set; }
    }

    public class ChangeFilmEnableCommandHandler : IRequestHandler<ChangeFilmEnableCommand, Result<long>>
    {
        private readonly IFilmRepository _filmRepository;
        private readonly IUnitOfWork<long> _unitOfWork;

        public ChangeFilmEnableCommandHandler(IFilmRepository filmRepository, IUnitOfWork<long> unitOfWork)
        {
            _filmRepository = filmRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(ChangeFilmEnableCommand request, CancellationToken cancellationToken)
        {
            var ExistFilm = _filmRepository.Entities.FirstOrDefault(_ => !_.IsDeleted && _.Id == request.FilmId);
            if (ExistFilm == null)
                return await Result<long>.FailAsync("NOT_FOUND_FILM");

            ExistFilm.Enable = !(ExistFilm.Enable);
            await _filmRepository.UpdateAsync(ExistFilm);
            await _unitOfWork.Commit(cancellationToken);

            return await Result<long>.SuccessAsync("Change film enable successfully");
        }
    }

}
