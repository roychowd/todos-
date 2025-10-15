using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// title of the todo item
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title must be less than 100 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// due date for the todo item
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        /// Indicates whether the todo item is completed default is false
        [Display(Name = "Completed")]
        public bool IsCompleted { get; set; } = false;

        /// created date for the todo item default is the current date
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    }
}


