using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Auth
{
    public record AuthResponse(
        string Token,
        Guid UserId,
        string Email,
        string FirstName,
        string LastName);
}
