using Application.Features.Schedule.Query.GetAll.GetAllByCinema;
using Application.Features.Schedule.Query.GetById;
using Application.Interfaces;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.FilmImage;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schedule.Query.GetAll.GetAllSchedule
{
    public class GetAllScheduleQuery: IRequest<Result<Dictionary<long, GetAllScheduleResponse>>>
    {
    }
    internal class GetAllScheduleHandler : IRequestHandler<GetAllScheduleQuery, Result<Dictionary<long, GetAllScheduleResponse>>>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IUploadService _uploadService;
        private readonly IFilmImageRepository _filmImageRepository;
        public GetAllScheduleHandler(IScheduleRepository scheduleRepository, 
            IFilmRepository filmRepository, 
            ICinemaRepository cinemaRepository, 
            IRoomRepository roomRepository,
            IUploadService uploadService,
            IFilmImageRepository filmImageRepository)
        {
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
            _uploadService = uploadService;
            _filmImageRepository = filmImageRepository;
        }
        public async Task<Result<Dictionary<long, GetAllScheduleResponse>>> Handle(GetAllScheduleQuery request, CancellationToken cancellationToken)
        {
            var result = new Dictionary<long, GetAllScheduleResponse>();
            var query = (from cinema in _cinemaRepository.Entities
                         join room in _roomRepository.Entities on cinema.Id equals room.CinemaId
                         join schedule in _scheduleRepository.Entities on room.Id equals schedule.RoomId
                         join film in _filmRepository.Entities on schedule.FilmId equals film.Id
                         where !cinema.IsDeleted && !room.IsDeleted && !film.IsDeleted && !schedule.IsDeleted
                         select new
                         {
                             schedule = schedule,
                             film = film,
                             room = room,
                             cinema = cinema
                         });
            var queryGroupByCinema = query.GroupBy(x => x.cinema.Id);
            foreach (var cinemaGroup in queryGroupByCinema)
            {
                var cinema = new GetAllScheduleResponse()
                {
                    City = cinemaGroup.First().cinema.City,
                    Name = cinemaGroup.First().cinema.Name,
                    Films = new Dictionary<long, GetAllScheduleFilmResponse>()
                };
                foreach (var scheduleInfo in cinemaGroup)
                {
                    var filmId = scheduleInfo.film.Id;
                    if (!cinema.Films.ContainsKey(filmId))
                    {
                        var film = new GetAllScheduleFilmResponse()
                        {
                            Name = scheduleInfo.film.Name,
                            Duration = scheduleInfo.film.Duration,
                            LimitAge = scheduleInfo.film.LimitAge,
                            StartDate = scheduleInfo.film.StartDate,
                            EndDate = scheduleInfo.film.EndDate,
                            Trailer = scheduleInfo.film.Trailer,
                            Image = _uploadService.GetFullUrl(_filmImageRepository.Entities.Where(_ => !_.IsDeleted && _.FilmId == scheduleInfo.film.Id).Select(y => y.NameFile).FirstOrDefault()),
                            Schedules = new List<GetAllScheduleScheduleResponse>()
                        };
                        cinema.Films.Add(filmId, film);
                    };
                    var schedule = new GetAllScheduleScheduleResponse()
                    {
                        Id = scheduleInfo.schedule.Id,
                        Duration = scheduleInfo.schedule.Duration,
                        StartTime = scheduleInfo.schedule.StartTime,
                        EndTime = scheduleInfo.schedule.StartTime.AddMinutes(scheduleInfo.schedule.Duration),
                        RoomId = scheduleInfo.schedule.RoomId,
                        Price = scheduleInfo.schedule.Price
                    };
                    cinema.Films[filmId].Schedules.Add(schedule);
                }
                result.Add(cinemaGroup.Key, cinema);
            }
            return await Result<Dictionary<long, GetAllScheduleResponse>>.SuccessAsync(result);
        }
    }
}
