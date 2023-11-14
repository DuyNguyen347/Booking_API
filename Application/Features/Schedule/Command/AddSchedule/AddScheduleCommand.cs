using Application.Exceptions;
using Application.Interfaces.Cinema;
using Application.Interfaces.Film;
using Application.Interfaces.Repositories;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.ScheduleSeat;
using Application.Interfaces.Seat;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Schedule.Command.AddSchedule
{
    public class AddScheduleCommand : IRequest<Result<List<ScheduleCommandResponse>>>
    {
        public long Id { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public long FilmId { get; set; }
        public long RoomId {  get; set; }
        public int Price { get; set; }
    }
    internal class AddScheduleCommandHandler : IRequestHandler<AddScheduleCommand, Result<List<ScheduleCommandResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IScheduleSeatRepository _scheduleSeatRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        public AddScheduleCommandHandler(IMapper mapper, IScheduleRepository scheduleRepository,
            IFilmRepository filmRepository,
            ICinemaRepository cinemaRepository,
            IRoomRepository roomRepository,
            ISeatRepository seatRepository,
            IScheduleSeatRepository scheduleSeatRepository,
            IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _roomRepository = roomRepository;
            _seatRepository = seatRepository;
            _scheduleSeatRepository = scheduleSeatRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<List<ScheduleCommandResponse>>> Handle(AddScheduleCommand request, CancellationToken cancellationToken)
        {
            var existFilm = await _filmRepository.FindAsync(x => x.Id == request.FilmId && !x.IsDeleted);
            var existRoom = await _roomRepository.FindAsync(x => x.Id == request.RoomId && !x.IsDeleted);
            if (existFilm == null) return await Result<List<ScheduleCommandResponse>>.FailAsync("NOT_FOUND_FILM");
            if (existRoom == null) return await Result<List<ScheduleCommandResponse>>.FailAsync("NOT_FOUND_ROOM");
            if (existRoom.Status != Domain.Constants.Enum.SeatStatus.Available) return await Result<List<ScheduleCommandResponse>>.FailAsync("ROOM_IS_NOT_AVAILABLE");
            if (request.Duration < existFilm.Duration) return await Result<List<ScheduleCommandResponse>>.FailAsync($"DURATION_SHORTER_THAN_FILM_DURATION({existFilm.Duration})");
            if (request.StartTime < DateTime.Now.AddMinutes(-1)) return await Result<List<ScheduleCommandResponse>>.FailAsync("NOT_VALID_TIME");
            List <ScheduleCommandResponse> listConflictSchedule = await IsScheduleConflict(request);
            if (listConflictSchedule.Count == 0)
            {
                var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var addSchedule = _mapper.Map<Domain.Entities.Schedule.Schedule>(request);
                    await _scheduleRepository.AddAsync(addSchedule);
                    await _unitOfWork.Commit(cancellationToken);
                    request.Id = addSchedule.Id;

                    var seatList = await _seatRepository.Entities.Where(x => x.RoomId == request.RoomId && !x.IsDeleted).ToListAsync();
                    List<Domain.Entities.ScheduleSeat.ScheduleSeat> scheduleSeats = new List<Domain.Entities.ScheduleSeat.ScheduleSeat>();
                    foreach (var seat in seatList)
                    {
                        scheduleSeats.Add(new Domain.Entities.ScheduleSeat.ScheduleSeat
                        {
                            SeatId = seat.Id,
                            ScheduleId = request.Id,
                            Status = Domain.Constants.Enum.SeatStatus.Available
                        });
                    }
                    await _scheduleSeatRepository.AddRangeAsync(scheduleSeats);
                    await _unitOfWork.Commit(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return await Result<List<ScheduleCommandResponse>>.SuccessAsync(new List<ScheduleCommandResponse>
                    {
                        new ScheduleCommandResponse
                        {
                            Id = request.Id,
                            Duration = request.Duration,
                            Description = request.Description,
                            StartTime = request.StartTime,
                            FilmId = request.FilmId,
                            RoomId = request.RoomId,
                            Price = request.Price
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    await transaction.RollbackAsync(cancellationToken);
                    throw new ApiException(ex.Message);
                }
                finally
                {
                    await transaction.DisposeAsync();
                }
            }
            return await Result<List<ScheduleCommandResponse>>.FailAsync(listConflictSchedule, "CONFLICT_WITH_EXISTING_SCHEDULE");
        }
        public async Task<List<ScheduleCommandResponse>> IsScheduleConflict(AddScheduleCommand request)
        {
            var listConflictSchedule = _scheduleRepository.Entities.AsEnumerable().Where(x => request.RoomId == x.RoomId && !x.IsDeleted).
                Where(x => (x.StartTime <= request.StartTime && request.StartTime < x.StartTime.AddMinutes(x.Duration)) ||
                (request.StartTime <= x.StartTime && x.StartTime < request.StartTime.AddMinutes(request.Duration))).
                AsQueryable().
                Select(x => new ScheduleCommandResponse
                {
                    Id = x.Id,
                    Duration = x.Duration,
                    Description = x.Description,
                    StartTime = x.StartTime,
                    FilmId = x.FilmId,
                    RoomId = x.RoomId,
                    Price = x.Price
                }).ToList();
            return listConflictSchedule;
        }
    }
}
