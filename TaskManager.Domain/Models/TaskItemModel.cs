
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Models
{
    public class TaskItemModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public TaskEnums Status { get; set; } = TaskEnums.New;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // FK -> Project (обязательный)
        public Guid ProjectId { get; set; }
        public ProjectModel Project { get; set; } = null!;

        // FK -> User (опциональный исполнитель)
        public Guid? AssigneeId { get; set; }
        public UserModel? Assignee { get; set; }
    }
}
