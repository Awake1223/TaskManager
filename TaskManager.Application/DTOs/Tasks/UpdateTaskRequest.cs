using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Tasks
{
    public record UpdateTaskRequest(
        string Name,
        string? Description,
        Guid? AssigneeId);
}
