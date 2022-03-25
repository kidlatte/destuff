using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class RegisterModel
{
    [Required]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }
}


public class RegisterResultModel
{
    public bool Succeeded { get; set; } = false;
    public IList<string> Errors { get; set; } = new List<string>();
}

public class LoginModel
{
    [Required]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }

    public bool Remember { get; set; }
}

public class AuthTokenModel
{
    public string? UserName { get; set; }
    public string? AuthToken { get; set; }
    public DateTime? Expires { get; set; }
}

public class PasswordChangeModel
{
    [Required]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }
}

public class UserModel
{
    public string? UserName { get; set; }
}