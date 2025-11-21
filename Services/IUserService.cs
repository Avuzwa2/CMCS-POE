using System.Threading.Tasks;
using CMCS_Web.Models;

namespace CMCS_Web.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User> CreateUserAsync(string email, string password, string role = "Lecturer", string? fullName = null);
        Task<User?> GetByEmailAsync(string email);
    }
}
