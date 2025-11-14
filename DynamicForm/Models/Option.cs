using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.Models
{
    public class Option
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        [Required]
        [StringLength(255)]
        public string Text { get; set; }

        public int OrderIndex { get; set; } = 0;

        // Navigation properties
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        public virtual ICollection<ResponseOption> ResponseOptions { get; set; } = new List<ResponseOption>();
    }
}
