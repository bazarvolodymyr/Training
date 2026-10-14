using MediatR;
using Training.Domain.Interfaces.Repositories;

namespace Training.Application.Features.Quiz.Queries.GetQuestion
{
    public class GetQuestionQueryHandler : IRequestHandler<GetQuestionQuery, GetQuestionResponse?>
    {
        private readonly IExerciseRepository _exerciseRepository;

        public GetQuestionQueryHandler(IExerciseRepository exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }

        public Task<GetQuestionResponse?> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
        {
            var exercise = _exerciseRepository.GetExercise(1);
            if (exercise is null)
            {
                return Task.FromResult<GetQuestionResponse?>(null);
            }

            return Task.FromResult<GetQuestionResponse?>(new GetQuestionResponse
            {
                Id = exercise.Id,
                Question = exercise.Question
            });
        }
    }
}
