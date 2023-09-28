using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class RegisterRequest
{
    [Required]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }
}

public class LoginRequest
{
    [Required]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }

    public bool Remember { get; set; }
}

public class PasswordRequest
{
    [Required]
    public required string UserName { get; set; }

    [Required]
    public string? Password { get; set; }
}

public class IdentityResultModel
{
    public bool Succeeded { get; set; } = false;
    public IList<string> Errors { get; set; } = new List<string>();
}

public class AuthModel
{
    public string? UserName { get; set; }
    public string? Token { get; set; }
    public DateTime? Expires { get; set; }
}
