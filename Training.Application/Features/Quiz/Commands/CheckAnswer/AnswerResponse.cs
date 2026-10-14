namespace Training.Application.Features.Quiz.Commands.CheckAnswer
{
    public record AnswerResponse
    {
        public int Id { get; set; }
        public string? Answer { get; set; }
    }
}
