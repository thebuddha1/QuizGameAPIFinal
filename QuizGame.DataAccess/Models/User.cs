using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Models;
public class User : IdentityUser
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    public Guid? RefreshToken { get; set; }
}
