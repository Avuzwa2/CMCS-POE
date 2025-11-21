using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CMCS_Web.Data;
using CMCS_Web.Models;

namespace CMCS_Web.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<User> _hasher;

        public UserService(ApplicationDbContext db, IPasswordHasher<User> hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<User> CreateUserAsync(string email, string password, string role = "Lecturer", string? fullName = null)
        {
            var existing = await _db.Users.AnyAsync(u => u.Email == email);
            if (existing) throw new InvalidOperationException("User already exists.");

            var user = new User
            {
                Email = email,
                Role = role,
                FullName = fullName
            };

            user.PasswordHash = _hasher.HashPassword(user, password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public Task<User?> GetByEmailAsync(string email)
            => _db.Users.SingleOrDefaultAsync(u => u.Email == email);
    }
}
