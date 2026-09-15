namespace Training.Domain.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
       // public string Anagram { get; set; } = string.Empty;
    }
}
