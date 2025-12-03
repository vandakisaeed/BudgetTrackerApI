using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Dtos.Users;

public record CreateUserDto(
    [property: Required]
    [property: StringLength(100, MinimumLength = 1)]
    string Name,
    string Email);