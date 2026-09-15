namespace Training.Application.Contracts
{
    public record AnswerResponse
    {
        public int Id {  get; set; }
        public string? Answer { get; set; }
    }
}
