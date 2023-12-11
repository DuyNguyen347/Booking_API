using Application.Features.Review.Command.CancelVote;
using Application.Features.Review.Command.Vote;
using Application.Features.Review.Query;
using Domain.Constants;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Review
{
    [ApiController]

    [Route("api/v{version:apiVersion}/review")]
    public class ReviewController: BaseApiController<ReviewController>
    {
        /// <summary>
        /// vote for a film by a customer
        /// </summary>
        [Authorize(Roles = RoleConstants.CustomerRole)]
        [HttpPost]
        public async Task<ActionResult<Result<VoteCommand>>> Vote(VoteCommand command)
        {
            var result = await Mediator.Send(command);
            return result.Succeeded? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// cancel vote
        /// </summary>
        [Authorize(Roles = RoleConstants.CustomerRole)]
        [HttpDelete]
        public async Task<ActionResult<Result<CancelVoteCommand>>> CancelVote(CancelVoteCommand command)
        {
            var result = await Mediator.Send(command);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get customer film vote
        /// </summary>
        [Authorize(Roles = RoleConstants.CustomerRole)]
        [HttpGet("{filmId}")]
        public async Task<ActionResult<Result<GetReviewByFilmResponse>>> GetReviewByFilm(long filmId)
        {
            var result = await Mediator.Send(new GetReviewByFilmQuery
            {
                FilmId = filmId
            });
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
