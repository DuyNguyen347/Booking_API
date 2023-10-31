using Application.Features.Category.Queries.GetAll;
using Application.Features.Film.Queries.GetAll;
using Application.Interfaces.Category;
using Application.Interfaces.Film;
using Application.Interfaces.Schedule;
using Application.Parameters;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schedule.Query.GetAll
{
    public class GetAllScheduleQuery: RequestParameter, IRequest<PaginatedResult<GetAllScheduleResponse>>
    {
    }
    //internal class GetAllScheduleHandler : IRequestHandler<GetAllScheduleQuery, PaginatedResult<GetAllScheduleResponse>>
    //{
    //    private readonly IScheduleRepository _scheduleRepository;
    //    private readonly IFilmRepository _filmRepository;

    //    public GetAllScheduleHandler(IScheduleRepository scheduleRepository, IFilmRepository filmRepository)
    //    {
    //        _scheduleRepository = scheduleRepository;
    //        _filmRepository = filmRepository;
    //    }
    //    public async Task<PaginatedResult<GetAllScheduleResponse>> Handle(GetAllScheduleQuery request, CancellationToken cancellationToken)
    //    {

    //    }
    //}
}
