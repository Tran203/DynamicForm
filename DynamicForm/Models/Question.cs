using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.Models
{
    public class Question
    {
        public int Id { get; set; }

        public int FormId { get; set; }

        [Required]
        [StringLength(500)]
        public string Text { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }  // Enum-like: Text, MultipleChoice, etc.

        public bool IsRequired { get; set; } = false;

        public int OrderIndex { get; set; } = 0;

        public int? MaxLength { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }

        // Navigation properties
        [ForeignKey("FormId")]
        public virtual Form Form { get; set; }

        public virtual ICollection<Option> Options { get; set; } = new List<Option>();
        public virtual ICollection<Response> Responses { get; set; } = new List<Response>();
    }
}
