using MediatR;
using Training.Domain.Enums;

namespace Training.Application.Features.Quiz.Queries.GetNextQuestion
{
    public record GetNextQuestionQuery(int LessonId, Difficulty Difficulty) : IRequest<NextQuestionResponse?>;

    public record NextQuestionResponse(
        int ExerciseId,
        int LessonId,
        string Question,
        string Difficulty,
        IReadOnlyList<string> Tokens);
}
