using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Ticket;
using Application.Interfaces.Seat;
using AutoMapper;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.Schedule;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Domain.Entities.Ticket;
using Domain.Constants;

namespace Application.Features.Ticket.Command
{
    public class AddTicketCommand : IRequest<Result<AddTicketCommand>>
    {
        public long Id { get; set; }
        public TypeTicket Type { get; set; }
        public long ScheduleId { get; set; }
        public List<long>? SeatId { get; set; }
    }

    internal class AddTicketCommandHandler : IRequestHandler<AddTicketCommand, Result<AddTicketCommand>>
    {
        private readonly IMapper _mapper;
        private readonly ITicketRepository _TicketRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ISeatRepository _seatReposity;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;


        public AddTicketCommandHandler(IMapper mapper, ITicketRepository TicketRepository, IUnitOfWork<long> unitOfWork, IScheduleRepository scheduleRepository, ISeatRepository seatReposity, ICurrentUserService currentUserService, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _TicketRepository = TicketRepository;
            _unitOfWork = unitOfWork;
            _seatReposity = seatReposity;
            _scheduleRepository = scheduleRepository;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<AddTicketCommand>> Handle(AddTicketCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine(_currentUserService);
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                long UserId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();
                var existSchedule = await _scheduleRepository.FindAsync(x => x.Id == request.ScheduleId && !x.IsDeleted);
                if (existSchedule == null) return await Result<AddTicketCommand>.FailAsync("NOT_FOUND_SCHEDULE");
                //var addTicket = _mapper.Map<Domain.Entities.Ticket.Ticket>(request);

                List<Domain.Entities.Ticket.Ticket> listTicket = request.SeatId.Select(seatid => new Domain.Entities.Ticket.Ticket
                {
                    Type = request.Type,
                    Price = existSchedule.Price,
                    UserId = UserId,
                    SeatId = seatid,
                    QRCode = null
                }).ToList();
                await _TicketRepository.AddRangeAsync(listTicket);
                await _unitOfWork.Commit(cancellationToken);

                // add seat to Ticket
                await transaction.CommitAsync(cancellationToken);

                return await Result<AddTicketCommand>.SuccessAsync(StaticVariable.SUCCESS);
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
    }
}
