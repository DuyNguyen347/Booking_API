using Application.Dtos.Requests.Feedback;
using Application.Exceptions;
using Application.Interfaces.CategoryFilm;
using Application.Interfaces.Film;
using Application.Interfaces.FilmImage;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Entities.CategoryFilm;
using Domain.Entities.FilmImage;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Film.Command.EditFilm
{
    public class EditFilmCommand : IRequest<Result<EditFilmCommand>>
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Actor { get; set; }
        public string? Director { get; set; }
        public string? Producer { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
        public int Year { get; set; }
        public string? Country { get; set; }
        public int LimitAge { get; set; }
        public string? Trailer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<long>? ListIdCategory { get; set; }
        public List<ImageRequest>? FileImages { get; set; }
        public string? Poster { get; set; }
    }

    internal class EditFilmCommandHandler : IRequestHandler<EditFilmCommand, Result<EditFilmCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IFilmRepository _filmRepository;
        private readonly IFilmImageRepository _filmImageRepository;
        private readonly ICategoryFilmRepository _categoryFilmRepository;

        public EditFilmCommandHandler(IMapper mapper, IFilmRepository filmRepository, IUnitOfWork<long> unitOfWork, IFilmImageRepository filmImageRepository, ICategoryFilmRepository categoryFilmRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _filmRepository = filmRepository;
            _filmImageRepository = filmImageRepository;
            _categoryFilmRepository = categoryFilmRepository;
        }

        public async Task<Result<EditFilmCommand>> Handle(EditFilmCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                return await Result<EditFilmCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var editFilm = await _filmRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
                if (editFilm == null) return await Result<EditFilmCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);

                _mapper.Map(request, editFilm);
                await _filmRepository.UpdateAsync(editFilm);

                List<CategoryFilm> oldListCateFilm = await _categoryFilmRepository.Entities.Where(x => x.FilmId ==  request.Id).ToListAsync();

                await _categoryFilmRepository.RemoveRangeAsync(oldListCateFilm);
            
                List<CategoryFilm> newlistCateFilm = request.ListIdCategory.Select(x => new CategoryFilm
                {
                    CategoryId = x,
                    FilmId = request.Id
                }).ToList();

                await _categoryFilmRepository.AddRangeAsync(newlistCateFilm);

                List<FilmImage> oldListImage = await _filmImageRepository.Entities.Where(x => x.FilmId == editFilm.Id && !x.IsDeleted).ToListAsync();

                await _filmImageRepository.RemoveRangeAsync(oldListImage);

                if (request.FileImages != null)
                {
                    var addImage = _mapper.Map<List<FilmImage>>(request.FileImages);
                    addImage.ForEach(x => x.FilmId = editFilm.Id);
                    await _filmImageRepository.AddRangeAsync(addImage);
                }

                await _unitOfWork.Commit(cancellationToken);
                request.Id = editFilm.Id;
                return await Result<EditFilmCommand>.SuccessAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine("err: ", ex);
                await transaction.RollbackAsync(cancellationToken);
                throw new ApiException(ex.Message);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
    }
}
