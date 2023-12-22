using Application.Exceptions;
using Application.Features.Schedule.Command.AddSchedule;
using Application.Interfaces.Film;
using Application.Interfaces.Repositories;
using Application.Interfaces.Room;
using Application.Interfaces.Schedule;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities.Schedule;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Schedule.Command.AddScheduleMultiTimeSlots
{
    public class AddScheduleMultiTimeSlotsCommand: IRequest<Result<List<ScheduleCommandResponse>>>
    {
        [Required]
        public long FilmId {  get; set; }
        [Required]
        public long RoomId { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public List<DateTime> StartTimes {  get; set; }
        public int Price { get; set; }
        public string? Description { get; set; }
    }
    public class AddScheduleMultipleTimeSlotsCommandHandler : IRequestHandler<AddScheduleMultiTimeSlotsCommand, Result<List<ScheduleCommandResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ITimeZoneService _timeZoneService;
        public AddScheduleMultipleTimeSlotsCommandHandler(
            IMapper mapper,
            IScheduleRepository scheduleRepository,
            IFilmRepository filmRepository,
            IRoomRepository roomRepository,
            IUnitOfWork<long> unitOfWork,
            ITimeZoneService timeZoneService)
        {
            _mapper = mapper;
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _roomRepository = roomRepository;
            _unitOfWork = unitOfWork;
            _timeZoneService = timeZoneService;
        }
        public async Task<Result<List<ScheduleCommandResponse>>> Handle(AddScheduleMultiTimeSlotsCommand request, CancellationToken cancellationToken)
        {
            var availableTimeToAdd = _timeZoneService.GetGMT7Time().AddMinutes(30);
            var existFilm = await _filmRepository.FindAsync(x => x.Id == request.FilmId && !x.IsDeleted);
            var existRoom = await _roomRepository.FindAsync(x => x.Id == request.RoomId && !x.IsDeleted);
            if (existFilm == null) return await Result<List<ScheduleCommandResponse>>.FailAsync("NOT_FOUND_FILM");
            if (existRoom == null) return await Result<List<ScheduleCommandResponse>>.FailAsync("NOT_FOUND_ROOM");
            if (existRoom.Status != Domain.Constants.Enum.SeatStatus.Available) return await Result<List<ScheduleCommandResponse>>.FailAsync("ROOM_IS_NOT_AVAILABLE");
            if (request.Duration < existFilm.Duration) return await Result<List<ScheduleCommandResponse>>.FailAsync($"DURATION_SHORTER_THAN_FILM_DURATION({existFilm.Duration})");

            List<ScheduleCommandResponse> InvalidStartTimes = new List<ScheduleCommandResponse>();
            foreach (var StartTime in request.StartTimes)
            {
                if (StartTime < availableTimeToAdd)
                    InvalidStartTimes.Add(new ScheduleCommandResponse
                    {
                        StartTime = StartTime
                    });
            }
            if (InvalidStartTimes.Count > 0)
                return await Result<List<ScheduleCommandResponse>>.FailAsync(InvalidStartTimes, "EXIST_NOT_VALID_START_TIME");

            if (CheckStartTimeConflict(request))
                return await Result<List<ScheduleCommandResponse>>.FailAsync("START_TIMES_CONFLICT_WITH_EACH_OTHER");

            List<ScheduleCommandResponse> listConflictSchedule = await GetListConflictExistingSchedule(request);
            if (listConflictSchedule.Count == 0)
            {
                var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var addScheduleList = new List<Domain.Entities.Schedule.Schedule>();
                    foreach (var StartTime in request.StartTimes)
                    {
                        addScheduleList.Add(new Domain.Entities.Schedule.Schedule
                        {
                            FilmId = request.FilmId,
                            RoomId = request.RoomId,
                            StartTime = StartTime,
                            Price = request.Price,
                            Description = request.Description,
                            Duration = request.Duration,
                        });
                    }
                    await _scheduleRepository.AddRangeAsync(addScheduleList);
                    await _unitOfWork.Commit(cancellationToken);

                    var result = new List<ScheduleCommandResponse>();
                    foreach (var schedule in addScheduleList)
                    {
                        result.Add(new ScheduleCommandResponse
                        {
                            Id = schedule.Id,
                            Duration = schedule.Duration,
                            Description = schedule.Description,
                            StartTime = schedule.StartTime,
                            FilmId = schedule.FilmId,
                            FilmName = existFilm.Name,
                            RoomId = schedule.RoomId,
                            Price = schedule.Price
                        }) ;
                    }

                    await transaction.CommitAsync(cancellationToken);

                    return await Result<List<ScheduleCommandResponse>>.SuccessAsync(result);
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
        public async Task<List<ScheduleCommandResponse>> GetListConflictExistingSchedule(AddScheduleMultiTimeSlotsCommand request)
        {
            List<ScheduleCommandResponse> result = new List<ScheduleCommandResponse>();
            foreach (var StartTime in request.StartTimes)
            {
                var listConflictSchedule = await _scheduleRepository.Entities.Where(x => request.RoomId == x.RoomId && !x.IsDeleted).
                        Where(x => (x.StartTime <= StartTime && StartTime < x.StartTime.AddMinutes(x.Duration)) ||
                        (StartTime <= x.StartTime && x.StartTime < StartTime.AddMinutes(request.Duration))).
                        Select(x => new ScheduleCommandResponse
                        {
                            Id = x.Id,
                            Duration = x.Duration,
                            Description = x.Description,
                            StartTime = x.StartTime,
                            FilmId = x.FilmId,
                            RoomId = x.RoomId,
                            Price = x.Price
                        }).ToListAsync();
                result.AddRange(listConflictSchedule);
            };
            result.ForEach(_ => _.FilmName = _scheduleRepository.GetFilmName(_.Id));
            return result;
        }
        public bool CheckStartTimeConflict(AddScheduleMultiTimeSlotsCommand request)
        {
            var sortedStartTime = request.StartTimes.Order().ToList();
            for (int i = 0; i < sortedStartTime.Count() - 1; i++)
            {
                if (sortedStartTime[i].AddMinutes(request.Duration) > sortedStartTime[i + 1])
                    return true;
            }
            return false;
        }
    }
}
