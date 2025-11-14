using System.ComponentModel.DataAnnotations;

namespace DynamicForm.Models
{
    public class Form
    {
        [Key]
        public int Id { get; set; }

        //[]
        public int? CompanyId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<Response> Responses { get; set; } = new List<Response>();
    }
}
