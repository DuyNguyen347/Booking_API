using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Constants;
using Application.Features.Statistics.Queries.GetFilmStatistic;
using Application.Features.Statistics.Queries.GetCinemaStatistic;

namespace WebApi.Controllers.V1.Statistics
{
    [ApiController]
    [Route("api/v{version:apiVersion}/statistics")]
    public class StatisticsController : BaseApiController<StatisticsController>
    {
        //    /// <summary>
        //    /// Get Insight Metrics
        //    /// </summary>
        //    /// <param name="statisticsTime"></param>
        //    /// <returns></returns>
        //    [Authorize(Roles = RoleConstants.AdministratorRole)]
        //    [HttpGet("insight-metrics")]
        //    public async Task<ActionResult<Result<GetInsightMetricsResponse>>> GetInsightMetrics(StatisticsTime statisticsTime)
        //    {
        //        return Ok(await Mediator.Send(new GetInsightMetricsQuery()
        //        {
        //            statisticsTime = statisticsTime
        //        }));
        //    }
        //    /// <summary>
        //    /// Get Overview
        //    /// </summary>
        //    /// <param name="statisticsTime"></param>
        //    /// <returns></returns>
        //    [Authorize(Roles = RoleConstants.AdministratorRole)]
        //    [HttpGet("overview")]
        //    public async Task<ActionResult<Result<List<GetOverviewResponse>>>> GetOverView(StatisticsTime statisticsTime)
        //    {
        //        return Ok(await Mediator.Send(new GetOverviewQuery()
        //        {
        //            statisticsTime = statisticsTime
        //        }));
        //    }
        //    /// <summary>
        //    /// Get Outstanding service
        //    /// </summary>
        //    /// <param name=""></param>
        //    /// <returns></returns>
        //    [Authorize(Roles = RoleConstants.AdministratorRole)]
        //    [HttpGet("outstanding-service")]
        //    public async Task<ActionResult<Result<List<GetOverviewResponse>>>> GetOutstandingService()
        //    {
        //        return Ok(await Mediator.Send(new GetOutstandingServiceQuery()
        //        {
        //        }));
        //    }

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
    }
}

