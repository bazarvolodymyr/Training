namespace Training.Application.Features.Quiz.Queries.GetQuestion
{
    public record GetQuestionResponse
    {
        public int Id { get; set; }
        public string? Question { get; set; }
    }
}
