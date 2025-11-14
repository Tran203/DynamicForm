using System.ComponentModel.DataAnnotations;

namespace DynamicForm.Models.ViewModels
{
    // Models/ViewModels/FormCreateViewModel.cs
    public class FormCreateViewModel
    {
        // ----- Form fields ----------------------------------------------------
        public int? Id { get; set; }
        public int? CompanyId { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        // ----- Questions (can be empty) --------------------------------------
        public List<QuestionCreateViewModel> Questions { get; set; } = new();
    }

    public class QuestionCreateViewModel
    {
        public int? Id { get; set; }                     // null = new
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = "Text";       // default
        public bool IsRequired { get; set; }
        public int OrderIndex { get; set; }

        // For numeric / rating
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }

        // Options – one per line (textarea) or individual rows
        public string OptionsText { get; set; } = string.Empty;

        // For edit mode – already persisted options
        public List<OptionEditViewModel> ExistingOptions { get; set; } = new();
    }

    public class OptionEditViewModel
    {
        public int? Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }
}
