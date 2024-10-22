using Application.Interfaces.Booking;
using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using Application.Interfaces;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Film;
using Application.Interfaces.FilmImage;
using Domain.Entities.FilmImage;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Film.Command.DeleteFilm
{
    public class DeleteFilmCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }

    }

    internal class DeleteFilmCommandHandler : IRequestHandler<DeleteFilmCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IFilmRepository _filmRepository;
        private readonly IFilmImageRepository _filmImageRepository;

        public DeleteFilmCommandHandler(
            IFilmRepository filmRepository,
            IFilmImageRepository filmImageCategory,
            IUnitOfWork<long> unitOfWork)
        {
            _filmImageRepository = filmImageCategory;
            _filmRepository = filmRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(DeleteFilmCommand request, CancellationToken cancellationToken)
        {
            var deleteFilm = await _filmRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (deleteFilm == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            await _filmRepository.DeleteAsync(deleteFilm);
            List<FilmImage> listFilmImage = await _filmImageRepository.Entities.Where(x => !x.IsDeleted && x.FilmId == deleteFilm.Id).ToListAsync();
            await _filmImageRepository.DeleteRange(listFilmImage);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<long>.SuccessAsync("Delete success");
        }
    }
}
