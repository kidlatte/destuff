using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

    public class AuthModel
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }
    }

    public class AuthTokenModel
    {
        public string? AuthToken { get; set; }
    }

    public class RegisterResultModel
    {
        public bool Succeeded { get; set; } = false;
        public IList<string> Errors { get; set; } = new List<string>();
    }
