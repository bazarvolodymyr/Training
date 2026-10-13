using MediatR;
using Training.Application.Contracts;

namespace Training.Application.Features.Quiz.Commands.CheckAnswer
{
    public record CheckAnswerCommand(int Id, string? Answer) : IRequest<ExerciseResult>;
}
