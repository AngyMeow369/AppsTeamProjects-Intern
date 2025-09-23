using System;
using System.ComponentModel.DataAnnotations;

namespace TaskLoggerV1.Models
{
    public class TaskModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime TaskDate { get; set; }

        [Required]
        public double HoursLogged { get; set; }

        [Required]
        public bool IsCompleted { get; set; }
    }
}
