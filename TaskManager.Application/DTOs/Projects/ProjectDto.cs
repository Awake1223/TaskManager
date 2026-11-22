using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Projects
{
    public record ProjectDto(
        Guid Id,
        string Name,
        string? Description,
        DateTime CreatedAt,
        Guid CreatedByUserId,
        string CreatedByUserName,
        int TasksCount);
}
