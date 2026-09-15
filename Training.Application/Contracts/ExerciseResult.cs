using System.Text.Json.Serialization;
using Training.Domain.Enums;

namespace Training.Application.Contracts
{
    public record ExerciseResult
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExerciseStatus ExerciseStatus { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? CorrectAnswer { get; set; }
    }
}
