using MediatR;
using Training.Domain.Interfaces.Repositories;
using GetQuestionDto = Training.Application.Contracts.GetQuestion;

namespace Training.Application.Features.Quiz.Queries.GetQuestion
{
    public class GetQuestionQueryHandler : IRequestHandler<GetQuestionQuery, GetQuestionDto?>
    {
        private readonly IExerciseRepository _exerciseRepository;

        public GetQuestionQueryHandler(IExerciseRepository exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }

        public Task<GetQuestionDto?> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
        {
            var exercise = _exerciseRepository.GetExercise(1);
            if (exercise is null)
            {
                return Task.FromResult<GetQuestionDto?>(null);
            }

            return Task.FromResult<GetQuestionDto?>(new GetQuestionDto
            {
                Id = exercise.Id,
                Question = exercise.Question
            });
        }
    }
}
