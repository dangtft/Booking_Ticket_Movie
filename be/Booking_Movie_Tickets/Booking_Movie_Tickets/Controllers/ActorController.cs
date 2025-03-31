using Booking_Movie_Tickets.DTOs.Actors;
using Booking_Movie_Tickets.DTOs.Actors.Response;
using Booking_Movie_Tickets.DTOs.Movies.Response;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Movies;
using Booking_Movie_Tickets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/actors")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActors([FromQuery] PagedFilterBase filter)
        {
            if (filter.Page < 1 || filter.PageSize < 1)
            {
                return BadRequest(new ApiResponse<string>(ApiMessages.INVALID_PAGINATION));
            }

            try
            {
                var pagedResult = await _actorService.GetAllAsync(filter);

                if (pagedResult == null || !pagedResult.Data.Any())
                {
                    return NoContent();
                }

                return Ok(new ApiResponse<Actor>(pagedResult));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(ApiMessages.ERROR));
            }
        }

        [HttpGet("{actorId}")]
        public async Task<IActionResult> GetActorById(Guid actorId)
        {
            var actor = await _actorService.GetByIdAsync(actorId);
            return actor == null
                ? NotFound(ApiResponse<string>.ErrorResponse(ApiMessages.NOT_FOUND))
                : Ok(ApiResponse<ActorDetailResponse>.SuccessResponse(actor, ApiMessages.SUCCESS));
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetActorsByMovieId(Guid movieId)
        {
            var actors = await _actorService.GetByMovieIdAsync(movieId);
            return actors.Any()
                ? Ok(ApiResponse<IEnumerable<ActorResponse>>.SuccessResponse(actors, ApiMessages.SUCCESS))
                : NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateActor([FromBody] Actor actor)
        {
            if (actor == null) return BadRequest(ApiResponse<string>.ErrorResponse(ApiMessages.ERROR));

            var createdActor = await _actorService.CreateAsync(actor);
            return CreatedAtAction(nameof(GetActorById), new { actorId = createdActor.Id },
                ApiResponse<Actor>.SuccessResponse(createdActor, ApiMessages.SUCCESS));
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPut("{actorId}")]
        public async Task<IActionResult> UpdateActor(Guid actorId, [FromBody] Actor actor)
        {
            if (actor == null) return BadRequest(ApiResponse<string>.ErrorResponse(ApiMessages.ERROR));

            actor.Id = actorId;
            var updated = await _actorService.UpdateAsync(actor);

            return updated
                ? Ok(ApiResponse<string>.SuccessResponse(ApiMessages.SUCCESS))
                : NotFound(ApiResponse<string>.ErrorResponse(ApiMessages.NOT_FOUND));
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpDelete("{actorId}")]
        public async Task<IActionResult> DeleteActor(Guid actorId)
        {
            var deleted = await _actorService.DeleteAsync(actorId);
            return deleted
                ? Ok(ApiResponse<string>.SuccessResponse(ApiMessages.SUCCESS))
                : NotFound(ApiResponse<string>.ErrorResponse(ApiMessages.NOT_FOUND));
        }
    }
}
