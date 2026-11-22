using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Tasks
{
    public record TaskDto(
        Guid Id,
        string Name,
        string? Description,
        TaskEnums Status,
        Guid ProjectId,
        string ProjectName,
        Guid? AssigneeId,
        string? AssigneeName,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
