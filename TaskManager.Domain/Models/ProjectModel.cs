using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Models;

namespace TaskManager.Domain.Models
{
    public class ProjectModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // FK -> User (создатель проекта)
        public Guid CreatedByUserId { get; set; }
        public UserModel CreatedByUser { get; set; } = null!;

        // Navigation
        public ICollection<TaskItemModel> Tasks { get; set; } = new List<TaskItemModel>();
    }
}
