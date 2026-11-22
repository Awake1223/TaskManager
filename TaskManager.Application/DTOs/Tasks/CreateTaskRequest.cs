using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Tasks
{
    public record CreateTaskRequest(
        string Name,
        string? Description,
        Guid ProjectId,
        Guid? AssigneeId);
}
