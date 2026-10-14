using MediatR;

namespace Training.Application.Features.Quiz.Commands.CheckAnswer
{
    public record CheckAnswerCommand(int Id, string? Answer) : IRequest<CheckAnswerResponse>;
}
