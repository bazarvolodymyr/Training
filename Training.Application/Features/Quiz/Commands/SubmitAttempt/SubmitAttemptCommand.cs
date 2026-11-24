using MediatR;
using Training.Application.Features.Quiz;
using Training.Domain.Enums;

namespace Training.Application.Features.Quiz.Commands.SubmitAttempt
{
    public record SubmitAttemptCommand(int ExerciseId, Difficulty Difficulty, string? Answer)
        : IRequest<SubmitAttemptResponse?>;

    public record SubmitAttemptResponse(
        string ExerciseStatus,
        string Question,
        string? Answer,
        string CorrectAnswer,
        IReadOnlyList<DiffSegment> Diff);
}
