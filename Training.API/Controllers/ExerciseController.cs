using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Training.API.Auth;
using Training.Application.Features.Quiz.Commands.SubmitAttempt;
using Training.Domain.Enums;

namespace Training.API.Controllers
{
    [Route("api/v1/exercises")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAntiforgery _antiforgery;

        public ExerciseController(IMediator mediator, IAntiforgery antiforgery)
        {
            _mediator = mediator;
            _antiforgery = antiforgery;
        }

        [EnableRateLimiting(TrainingRateLimits.Submit)]
        [HttpPost("{id:int}/attempts")]
        public async Task<IActionResult> Submit(int id, SubmitAttemptRequest? request, CancellationToken cancellationToken)
        {
            if (!await IsRequestValidAsync())
            {
                return BadRequest();
            }

            if (request is null || !Enum.TryParse<Difficulty>(request.Difficulty, ignoreCase: true, out var level))
            {
                return BadRequest();
            }

            var result = await _mediator.Send(new SubmitAttemptCommand(id, level, request.Answer), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        private async Task<bool> IsRequestValidAsync()
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
                return true;
            }
            catch (AntiforgeryValidationException)
            {
                return false;
            }
        }
    }

    public record SubmitAttemptRequest(string? Difficulty, string? Answer);
}
