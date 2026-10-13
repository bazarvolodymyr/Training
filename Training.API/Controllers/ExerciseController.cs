using MediatR;
using Microsoft.AspNetCore.Mvc;
using Training.Application.Contracts;
using Training.Application.Features.Quiz.Commands.CheckAnswer;
using Training.Application.Features.Quiz.Queries.GetQuestion;

namespace Training.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExerciseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestion(CancellationToken cancellationToken)
        {
            var question = await _mediator.Send(new GetQuestionQuery(), cancellationToken);
            return question is null ? NotFound() : Ok(question);
        }

        [HttpPost]
        public async Task<IActionResult> QuestionResponse(AnswerResponse answer, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CheckAnswerCommand(answer.Id, answer.Answer), cancellationToken);
            return Ok(result);
        }
    }
}
