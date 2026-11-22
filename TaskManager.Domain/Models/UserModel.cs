using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TaskManager.Domain.Models;

namespace TaskManager.Domain.Models
{
    public class UserModel : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        /// <summary>Soft-delete. Пользователей физически не удаляем.</summary>
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<TaskItemModel> AssignedTasks { get; set; } = new List<TaskItemModel>();
        public ICollection<ProjectModel> CreatedProjects { get; set; } = new List<ProjectModel>();
    }
}
