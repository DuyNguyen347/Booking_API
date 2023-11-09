using Application.Features.Room.Queries.GetAll;
using Application.Interfaces.Room;
using Application.Parameters;
using Domain.Constants.Enum;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;


namespace Application.Features.Room.Queries.GetAll
{
    public class GetAllRoomQuery : RequestParameter, IRequest<PaginatedResult<GetAllRoomResponse>>
    {
        public SeatStatus Status { get; set; }
    }
    public class GetAllRoomHandler : IRequestHandler<GetAllRoomQuery, PaginatedResult<GetAllRoomResponse>>
    {
        private readonly IRoomRepository _RoomRepository;
        public GetAllRoomHandler(IRoomRepository RoomRepository)
        {
            _RoomRepository = RoomRepository;
        }
        public async Task<PaginatedResult<GetAllRoomResponse>> Handle(GetAllRoomQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = _RoomRepository.Entities.AsEnumerable()
                        .Where(x => !x.IsDeleted
                                && (string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(x.Name, request.Keyword)))
                        .AsQueryable()
                        .Select(x => new GetAllRoomResponse
                        {
                            Id = x.Id,
                            Name = x.Name,
                            NumberSeat =x.NumberSeat,
                            Status = x.Status,
                            CinemaId = x.CinemaId,
                            CreatedOn = x.CreatedOn,
                            LastModifiedOn = x.LastModifiedOn
                        });
            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllRoomResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllRoomResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}
