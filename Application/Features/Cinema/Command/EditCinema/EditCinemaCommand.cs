using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.Cinema;
using Domain.Entities.CinemaImage;
using Application.Interfaces.CinemaImage;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;

namespace Application.Features.Cinema.Command.EditCinema
{
    public class EditCinemaCommand : IRequest<Result<EditCinemaCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? City { get; set; }
        public string? Hotline { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? Address { get; set; }
        public List<string>? listImage { get; set; }
    }

    internal class EditCinemaCommandHandler : IRequestHandler<EditCinemaCommand, Result<EditCinemaCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly ICinemaImageRepository _cinemaImageRepository;
        private readonly IUploadService _uploadService;

        public EditCinemaCommandHandler(IMapper mapper, ICinemaRepository cinemaRepository, IUnitOfWork<long> unitOfWork, ICinemaImageRepository cinemaImageRepository, IUploadService uploadService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cinemaRepository = cinemaRepository;
            _cinemaImageRepository = cinemaImageRepository;
            _uploadService = uploadService;
        }

        public async Task<Result<EditCinemaCommand>> Handle(EditCinemaCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                return await Result<EditCinemaCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            var editCinema = await _cinemaRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (editCinema == null) return await Result<EditCinemaCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            
            _mapper.Map(request, editCinema);
            await _cinemaRepository.UpdateAsync(editCinema);

            List<CinemaImage> cinemaImages = await _cinemaImageRepository.Entities.Where(x => x.CinemaId == editCinema.Id && !x.IsDeleted).ToListAsync();
            foreach(var i in cinemaImages)
            {
                await _uploadService.DeleteAsync(i.NameFile);
            }
            await _cinemaImageRepository.RemoveRangeAsync(cinemaImages);

            request.Id = editCinema.Id;
            if (request.listImage != null)
            {
                List<CinemaImage> newListCinemaImage = request.listImage.Select(x => new CinemaImage
                {
                    CinemaId = request.Id,
                    NameFile = x
                }).ToList();
                await _cinemaImageRepository.AddRangeAsync(newListCinemaImage);
            }

            await _unitOfWork.Commit(cancellationToken);
            return await Result<EditCinemaCommand>.SuccessAsync(request);
        }
    }
}
