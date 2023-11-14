using Application.Features.Film.Command.DeleteFilm;
using Application.Interfaces.Film;
using Application.Interfaces.FilmImage;
using Application.Interfaces.Repositories;
using Application.Interfaces.Schedule;
using Application.Interfaces.ScheduleSeat;
using Domain.Constants;
using Domain.Entities.FilmImage;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schedule.Command.DeleteSchedule
{
    public class DeleteScheduleCommand: IRequest<Result<long>>
    {
        public long Id { get; set; }
    }
    internal class DeleteScheduleCommandHandler: IRequestHandler<DeleteScheduleCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleSeatRepository _scheduleSeatRepository;
        public DeleteScheduleCommandHandler(
            IUnitOfWork<long> unitOfWork, 
            IScheduleRepository scheduleRepository,
            IScheduleSeatRepository scheduleSeatRepository)
        {
            _unitOfWork = unitOfWork;
            _scheduleRepository = scheduleRepository;
            _scheduleSeatRepository = scheduleSeatRepository;
        }
        public async Task<Result<long>> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
        {
            var deleteSchedule = await _scheduleRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (deleteSchedule == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            await _scheduleRepository.DeleteAsync(deleteSchedule);

            var scheduleSeats = await _scheduleSeatRepository.Entities.Where(x=> x.ScheduleId == request.Id).ToListAsync();
            await _scheduleSeatRepository.DeleteRange(scheduleSeats);

            await _unitOfWork.Commit(cancellationToken);

            return await Result<long>.SuccessAsync("Delete success");
        }

    }
}
