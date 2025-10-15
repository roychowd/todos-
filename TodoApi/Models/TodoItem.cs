using System.ComponentModel.DataAnnotations;

// 
// This is the model for the todo item  
// id - a guid
// title - a string of up to 100 characters
// description - a string
// due date - a date
// is completed - a boolean default is false
// created at - a date default is the current date

namespace TodoApi.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title must be less than 100 characters")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}


