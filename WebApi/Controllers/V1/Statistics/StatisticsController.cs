using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Constants;
using Application.Features.Statistics.Queries.GetFilmStatistic;
using Application.Features.Statistics.Queries.GetCinemaStatistic;
using Application.Features.Statistics.Queries.GetDaytimeRangesStatistic;
using Application.Features.Statistics.Queries.GetStatisticByTimeStep;

namespace WebApi.Controllers.V1.Statistics
{
    [ApiController]
    [Route("api/v{version:apiVersion}/statistics")]
    public class StatisticsController : BaseApiController<StatisticsController>
    {
        /// <summary>
        /// Get film statistic
        /// </summary>
        /// <param name="TimeOption">Time option, 0: today, 1: this week, 2: this month, 3: this year, 4: custom time</param>
        /// <param name="FromTime">if TimeOption is 4; fill this field</param>
        /// <param name="ToTime">if TimeOption is 4; fill this field</param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpGet("film")]
        public async Task<ActionResult<PaginatedResult<GetFilmStatisticResponse>>> GetFilmStatistic([FromQuery] GetFilmStatisticQuery query)
        {
            return Ok(await Mediator.Send(new GetFilmStatisticQuery
            {
                TimeOption = query.TimeOption,
                FromTime = query.FromTime,
                ToTime = query.ToTime,
                FilmId = query.FilmId,
                IsExport = query.IsExport,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Keyword = query.Keyword,
                OrderBy = query.OrderBy,
            }));
        }

        /// <summary>
        /// Get cinema statistic
        /// </summary>
        /// <param name="TimeOption">Time option, 0: today, 1: this week, 2: this month, 3: this year, 4: custom time</param>
        /// <param name="FromTime">if TimeOption is 4; fill this field</param>
        /// <param name="ToTime">if TimeOption is 4; fill this field</param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpGet("cinema")]
        public async Task<ActionResult<PaginatedResult<GetCinemaStatisticResponse>>> GetCinemaStatistic([FromQuery] GetCinemaStatisticQuery query)
        {
            return Ok(await Mediator.Send(new GetCinemaStatisticQuery
            {
                TimeOption = query.TimeOption,
                FromTime = query.FromTime,
                ToTime = query.ToTime,
                CinemaId = query.CinemaId,
                IsExport = query.IsExport,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Keyword = query.Keyword,
                OrderBy = query.OrderBy,
            }));
        }

        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpGet("daytime-ranges")]
        public async Task<ActionResult<Result<List<GetDaytimeRangesStatisticResponse>>>> GetDaytimeRangesStatistic([FromQuery] GetDaytimeRangesStatisticQuery query)
        {
            return Ok(await Mediator.Send(new GetDaytimeRangesStatisticQuery
            {
                TimeOption = query.TimeOption,
                FromTime = query.FromTime,
                ToTime = query.ToTime,
                OrderBy = query.OrderBy
            }));
        }

        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpGet("time-step")]
        public async Task<ActionResult<PaginatedResult<GetStatisticByTimeStepResponse>>> GetStatisticByTimeStep([FromQuery] GetStatisticByTimeStepQuery query)
        {
            var result = await Mediator.Send(new GetStatisticByTimeStepQuery
            {
                TimeStep = query.TimeStep,
                IsExport = query.IsExport,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                CinemaId = query.CinemaId,
            });
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}

