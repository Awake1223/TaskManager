
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Tasks
{
    public record UpdateTaskStatusRequest(TaskEnums Status);
}
