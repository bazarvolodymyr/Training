using MediatR;

namespace Training.Application.Features.Quiz.Queries.GetQuestion
{
    public record GetQuestionQuery : IRequest<GetQuestionResponse?>;
}
