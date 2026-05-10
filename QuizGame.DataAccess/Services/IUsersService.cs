using QuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Services;
public interface IUsersService
{
    Task AddUserAsync(User user, string password, Role? role = null);
    Task<(string authToken, string refreshToken, string userId)> LoginAsync(string email, string password);
    Task<(string authToken, string refreshToken, string userId)> RedeemRefreshTokenAsync(string refreshToken);
    Task LogoutAsync();
    Task<User?> GetCurrentUserAsync();
    string? GetCurrentUserId();
    Task<User> GetUserByIdAsync(string id);
    List<Role> GetCurrentUserRoles();
    bool IsCurrentUserAdmin();
}