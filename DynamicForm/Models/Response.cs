using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.Models
{
    public class Response
    {
        public int Id { get; set; }

        public int FormId { get; set; }
        public int QuestionId { get; set; }
        public int RespondentId { get; set; }

        //public string RespondentEmail { get; set; }

        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        public string? AnswerText { get; set; }     // For text-based answers
        public int? AnswerValue { get; set; }      // For numeric/rating

        // Navigation properties
        //[ForeignKey("FormId")]
        public virtual Form Form { get; set; }

        //[ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        public virtual ICollection<ResponseOption> ResponseOptions { get; set; } = new List<ResponseOption>();
    }
}

