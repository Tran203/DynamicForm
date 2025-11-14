using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.Models
{
    public class ResponseOption
    {
        public int Id { get; set; }

        public int ResponseId { get; set; }
        public int OptionId { get; set; }

        // Navigation properties
        //[ForeignKey("ResponseId")]
        public virtual Response Response { get; set; }

        //[ForeignKey("OptionId")]
        public virtual Option Option { get; set; }
    }
}
