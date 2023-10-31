using Application.Interfaces.Film;
using Application.Interfaces.Repositories;
using Application.Interfaces.Schedule;
using AutoMapper;
using Domain.Entities.Schedule;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schedule.Command.AddSchedule
{
    public class AddScheduleCommand : IRequest<Result<AddScheduleCommand>>
    {
        public long Id { get; set; }
        public string Description { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public long FilmId { get; set; }
        public long RoomId {  get; set; }
        public int Price { get; set; }
    }
    internal class AddScheduleCommandHandler : IRequestHandler<AddScheduleCommand, Result<AddScheduleCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        public AddScheduleCommandHandler(IMapper mapper, IScheduleRepository scheduleRepository, IFilmRepository filmRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _scheduleRepository = scheduleRepository;
            _filmRepository = filmRepository;
            _ro
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<AddScheduleCommand>> Handle(AddScheduleCommand request, CancellationToken cancellationToken)
        {
            var addSchedule = _mapper.Map<Domain.Entities.Schedule.Schedule>(request);
            var isConflict = await IsScheduleConflict(request);
            if (isConflict == null) 
            {
                await _scheduleRepository.AddAsync(addSchedule);
                await _unitOfWork.Commit(cancellationToken);
                request.Id = addSchedule.Id;
                return await Result<AddScheduleCommand>.SuccessAsync(request);
            }
            return await Result<AddScheduleCommand>.FailAsync("Conflict schedule");
        }
        public async Task<Domain.Entities.Schedule.Schedule> IsScheduleConflict(AddScheduleCommand request )
        {
            //var conflict = await _scheduleRepository.Entities.
            //    Where(x => x.RoomId == newSchedule.RoomId).
            //    Where(x => (newSchedule.StartTime >= x.StartTime && newSchedule.StartTime <=x.StartTime.AddMinutes(GetFilmDuration(x.FilmId)))||
            //    (newSchedule.StartTime<=x.StartTime && newSchedule.StartTime.AddMinutes(GetFilmDuration(newSchedule.FilmId))>=x.StartTime)).ToListAsync();
            //return conflict.Any();
            return null;
        }
        //public async Task<double> GetFilmDuration(long filmId)
        //{
        //    var film = await _filmRepository.Entities.FirstOrDefaultAsync(x => x.Id == filmId);
        //    return film != null? film.Duration : 0;
        //}

    }

}
