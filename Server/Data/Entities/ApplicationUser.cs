using Destuff.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace Destuff.Server.Data.Entities;

public class ApplicationUser: IdentityUser
{
    public UserSettings? Settings { get; set; } 
}
