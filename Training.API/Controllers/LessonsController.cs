using MediatR;
using Microsoft.AspNetCore.Mvc;
using Training.Application.Features.Lessons.Queries.GetLesson;
using Training.Application.Features.Lessons.Queries.ListLessons;
using Training.Application.Features.Quiz.Queries.GetNextQuestion;
using Training.Domain.Enums;

namespace Training.API.Controllers
{
    [Route("api/v1/lessons")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LessonsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            var lessons = await _mediator.Send(new ListLessonsQuery(), cancellationToken);
            return Ok(lessons);
        }

        [HttpGet("{id:int}/questions/next")]
        public async Task<IActionResult> Next(int id, string? difficulty, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<Difficulty>(difficulty, ignoreCase: true, out var level))
            {
                return BadRequest();
            }

            var question = await _mediator.Send(new GetNextQuestionQuery(id, level), cancellationToken);
            return question is null ? NotFound() : Ok(question);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var lesson = await _mediator.Send(new GetLessonQuery(id), cancellationToken);
            return lesson is null ? NotFound() : Ok(lesson);
        }
    }
}
