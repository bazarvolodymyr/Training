using MediatR;
using GetQuestionDto = Training.Application.Contracts.GetQuestion;

namespace Training.Application.Features.Quiz.Queries.GetQuestion
{
    public record GetQuestionQuery : IRequest<GetQuestionDto?>;
}
