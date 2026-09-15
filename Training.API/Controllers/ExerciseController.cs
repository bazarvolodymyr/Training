using Microsoft.AspNetCore.Mvc;
using Training.Application.Contracts;
using Training.Domain.Interfaces.Services;

namespace Training.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        public ExerciseController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }
        [HttpGet]
        public IActionResult GetQuestion()
        {
            if(_exerciseService.GetExercise()==null)
            {
                return NotFound();
            }
            return Ok(_exerciseService.GetExercise());
        }
        [HttpPost]
        public IActionResult QuestionResponse(AnswerResponse answer)
        {
            return Ok(_exerciseService.CheckAnswer(answer));
        }
    }
}
