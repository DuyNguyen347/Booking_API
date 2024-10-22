using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.Cinema;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.CinemaImage;
using Application.Interfaces;

namespace Application.Features.Cinema.Queries.GetById
{
    public class GetCinemaByIdQuery : IRequest<Result<GetCinemaByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetCinemaByIdQueryHandler : IRequestHandler<GetCinemaByIdQuery, Result<GetCinemaByIdResponse>>
    {
        private readonly ICinemaRepository _cinemaRepository;
        private readonly ICinemaImageRepository _cinemaImageRepository;
        private readonly IUploadService _uploadService;

        public GetCinemaByIdQueryHandler(ICinemaRepository cinemaRepository, ICinemaImageRepository cinemaImageRepository, IUploadService uploadService)
        {
            _cinemaRepository = cinemaRepository;
            _cinemaImageRepository = cinemaImageRepository;
            _uploadService = uploadService;
        }

        public async Task<Result<GetCinemaByIdResponse>> Handle(GetCinemaByIdQuery request, CancellationToken cancellationToken)
        {
            var cinema = await (from e in _cinemaRepository.Entities
                                  where e.Id == request.Id && !e.IsDeleted
                                  select new GetCinemaByIdResponse()
                                  {
                                      Id = e.Id,
                                      Name = e.Name,
                                      Description = e.Description,
                                      City = e.City,
                                      Hotline = e.Hotline,
                                      Latitude = e.Latitude,
                                      Longitude = e.Longitude,
                                      Address = e.Address
                                  }).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (cinema == null) return await Result<GetCinemaByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            cinema.listImage = GetListUrlImage(cinema.Id);
            return await Result<GetCinemaByIdResponse>.SuccessAsync(cinema);
        }
        public List<string> GetListUrlImage(long idCinema)
        {
            var listUrl = _cinemaImageRepository.Entities.Where(_ => !_.IsDeleted && _.CinemaId == idCinema).Select(y => y.NameFile).ToList();
            List<string> listFullUrl = new List<string>();
            foreach (string? i in listUrl)
            {
                listFullUrl.Add(_uploadService.GetFullUrl(i));
            }
            return listFullUrl;
        }
    }
}
