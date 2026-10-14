using System.Text.Json.Serialization;
using Training.Domain.Enums;

namespace Training.Application.Features.Quiz.Commands.CheckAnswer
{
    public record CheckAnswerResponse
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExerciseStatus ExerciseStatus { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? CorrectAnswer { get; set; }
    }
}
