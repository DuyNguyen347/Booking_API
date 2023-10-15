using Application.Dtos.Requests.Feedback;
using Application.Exceptions;
using Application.Features.Booking.Command.AddBooking;
using Application.Features.Service.Command.AddService;
using Application.Interfaces.Category;
using Application.Interfaces.CategoryFilm;
using Application.Interfaces.Film;
using Application.Interfaces.FilmImage;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using AutoMapper;
using Domain.Entities.CategoryFilm;
using Domain.Entities.FilmImage;
using Domain.Entities.ServiceImage;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Film.Command.AddFilm
{
    public class AddFilmCommand : IRequest<Result<AddFilmCommand>>
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Actor { get; set; }
        public string? Director { get; set; }
        public string? Producer { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
        public int Year { get; set; }
        public string?  Country { get; set; }
        public int  LimitAge { get; set; }
        public string? Trailer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get ; set; }
        public List<long> ListIdCategory { get; set; }
        public List<ImageRequest> FileImages { get; set; }

    }

    internal class AddFilmCommandHandler : IRequestHandler<AddFilmCommand, Result<AddFilmCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IFilmRepository _filmRepository;
        private readonly IFilmImageRepository _filmImageRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryFilmRepository _categoryFilmRepository;
        private readonly IUnitOfWork<long> _unitOfWork;

        public AddFilmCommandHandler(IMapper mapper, IFilmRepository filmRepository, IFilmImageRepository filmImageRepository,ICategoryRepository categoryRepository, ICategoryFilmRepository categoryFilmRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _filmRepository = filmRepository;
            _filmImageRepository = filmImageRepository;
            _categoryRepository = categoryRepository;
            _categoryFilmRepository = categoryFilmRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AddFilmCommand>> Handle(AddFilmCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                List<long> ListIdCate = await _categoryRepository.Entities.Where(x => !x.IsDeleted).Select(x => x.Id).ToListAsync();
                if (request.ListIdCategory.Except(ListIdCate).ToList().Any()) 
                    return await Result<AddFilmCommand>.FailAsync("Not Found Category");

                var addFilm = _mapper.Map<Domain.Entities.Films.Film>(request);
                await _filmRepository.AddAsync(addFilm);
                await _unitOfWork.Commit(cancellationToken);
                request.Id = addFilm.Id;

                List<CategoryFilm> listCateFilm = request.ListIdCategory.Select(x => new CategoryFilm
                {
                    CategoryId = x,
                    FilmId = addFilm.Id
                }).ToList();

                await _categoryFilmRepository.AddRangeAsync(listCateFilm);
                await _unitOfWork.Commit(cancellationToken);

                if (request.FileImages != null)
                {
                    var addImage = _mapper.Map<List<FilmImage>>(request.FileImages);
                    addImage.ForEach(x => x.FilmId = addFilm.Id);
                    await _filmImageRepository.AddRangeAsync(addImage);
                    await _unitOfWork.Commit(cancellationToken);
                }

                await transaction.CommitAsync();
                return await Result<AddFilmCommand>.SuccessAsync(request);
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
