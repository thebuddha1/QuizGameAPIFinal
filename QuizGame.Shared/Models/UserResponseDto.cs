using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.Shared.Models;
public record UserResponseDto
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string PhoneNumber { get; init; }

    public required string Email { get; init; }
}
