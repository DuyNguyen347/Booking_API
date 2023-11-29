using Application.Features.Schedule.Command.AddSchedule;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Repositories;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Schedule.Command.AddScheduleCinemas
{
    public class AddScheduleMultipleCinemasCommand: IRequest<Result<AddScheduleMultipleCinemasResponse>>
    {
        public long Id { get; set; }
        public int Duration { get; set; }
        public DateTime StartTime {  get; set; }
        public long FilmId { get; set; }
        public List<long>? ListCinemaId {  get; set; } 
        public int Price { get; set; }
        public string? Description { get; set; }
    }
    internal class AddScheduleMultipleCinemasCommandHandler: IRequestHandler<AddScheduleMultipleCinemasCommand, Result<AddScheduleMultipleCinemasResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ITimeZoneService _timeZoneService;
        public AddScheduleMultipleCinemasCommandHandler(
            IMapper mapper,
            IScheduleRepository scheduleRepository,
            IFilmRepository filmRepository,
            ICinemaRepository cinemaRepository,
            IRoomRepository roomRepository,
            IUnitOfWork<long> unitOfWork,
            ITimeZoneService timeZoneService)
        {
            _mapper = mapper;
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _unitOfWork = unitOfWork;
            _cinemaRepository = cinemaRepository;
            _roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
            _timeZoneService = timeZoneService;
        }
        public async Task<Result<AddScheduleMultipleCinemasResponse>> Handle(AddScheduleMultipleCinemasCommand request, CancellationToken cancellationToken)
        {
            var existFilm = await _filmRepository.FindAsync(x => x.Id == request.FilmId && !x.IsDeleted);         
            if (existFilm == null) return await Result<AddScheduleMultipleCinemasResponse>.FailAsync("NOT_FOUND_FILM");
            if (request.Duration < existFilm.Duration) return await Result<AddScheduleMultipleCinemasResponse>.FailAsync($"DURATION_SHORTER_THAN_FILM_DURATION({existFilm.Duration})");
            if (request.StartTime < _timeZoneService.GetGMT7Time().AddMinutes(30)) return await Result<AddScheduleMultipleCinemasResponse>.FailAsync("NOT_VALID_TIME");
            AddScheduleMultipleCinemasResponse response = new AddScheduleMultipleCinemasResponse()
            {
                Duration = request.Duration,
                StartTime = request.StartTime,
                Price = request.Price,
                Description = request.Description
            };

            Dictionary<long, long> AddableCinemas = new Dictionary<long, long>();
            Dictionary<long, long> UnaddableCinemas = new Dictionary<long, long>();

            foreach (var CinemaId in request.ListCinemaId)
            {
                var CinemaRooms = await (from room in _roomRepository.Entities
                                         where room.CinemaId == CinemaId && room.Status == Domain.Constants.Enum.SeatStatus.Available
                                         select room).ToListAsync();
                if (CinemaRooms.Count == 0)
                    UnaddableCinemas[CinemaId] = -1;
                else {
                    var UnavailableRoom = await (from room in _roomRepository.Entities
                                                 where room.Status == Domain.Constants.Enum.SeatStatus.Available
                                                 && room.CinemaId == CinemaId && !room.IsDeleted
                                                 join schedule in _scheduleRepository.Entities
                                                 on room.Id equals schedule.RoomId
                                                 where !schedule.IsDeleted
                                                 where (schedule.StartTime <= request.StartTime && request.StartTime < schedule.StartTime.AddMinutes(schedule.Duration))
                                                 || (request.StartTime <= schedule.StartTime && schedule.StartTime < request.StartTime.AddMinutes(request.Duration))
                                                 select room).ToListAsync();
                    var AvailableRoom = CinemaRooms.Except(UnavailableRoom).FirstOrDefault();
                    if (AvailableRoom != null) AddableCinemas[CinemaId] = AvailableRoom.Id;
                    else UnaddableCinemas[CinemaId] = 0;
                }
            }
            if (UnaddableCinemas.Count > 0)
            {
                foreach (var (cinema, room) in UnaddableCinemas)
                {
                    response.CinemaSchedules.Add(new CinemaSchedule
                    {
                        CinemaId = cinema,
                        RoomId = room,
                        CinemaName = _cinemaRepository.Entities.First(x => x.Id == cinema).Name,
                    });
                }
                return await Result<AddScheduleMultipleCinemasResponse>.FailAsync(response, "There are cinemas with no rooms (roomId: -1) or rooms with showings scheduled (roomId: 0)");
            }
            foreach (var (cinema, room) in AddableCinemas)
            {
                var addSchedule = new Domain.Entities.Schedule.Schedule
                {
                    Duration = request.Duration,
                    Description = request.Description,
                    StartTime = request.StartTime,
                    FilmId = request.FilmId,
                    RoomId = room,
                    Price = request.Price
                };
                await _scheduleRepository.AddAsync(addSchedule);
                await _unitOfWork.Commit(cancellationToken);
                response.CinemaSchedules.Add(new CinemaSchedule
                {
                    Id = addSchedule.Id,
                    CinemaId = cinema,
                    RoomId = room,
                    CinemaName = _cinemaRepository.Entities.First(x => x.Id == cinema).Name,
                });
            }
            return await Result<AddScheduleMultipleCinemasResponse>.SuccessAsync(response);
        }
    }
}
