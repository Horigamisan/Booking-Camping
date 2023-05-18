using FinalProject_API.Models;
using System.Security.Cryptography;

namespace FinalProject_API.Services
{
    public interface IAuthService
    {
        public User? FindByEmailAsync(string email);
        public User? FindByIdAsync(int id);
        public bool ConfirmEmailAsync(int userId, string token);
        public string GenerateEmailConfirmationTokenAsync(int userId);

    }
    public class AuthService : IAuthService
    {
        private readonly FinalProject_SOAContext db;
        public AuthService(FinalProject_SOAContext db)
        {
            this.db = db;
        }

        public bool ConfirmEmailAsync(int userId, string token)
        {
            var user = db.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                throw new ArgumentException("Invalid user id");
            }

            if ((bool)user.Confirmed)
            {
                return true; // Email đã được xác nhận
            }

            if (user.ConfirmCode == null || user.ConfirmCode != token)
            {
                return false; // Token không hợp lệ
            }

            // Cập nhật trạng thái xác nhận email của người dùng
            user.Confirmed = true;
            user.ConfirmCode = null;
            db.SaveChanges();

            return true;
        }

        public User? FindByEmailAsync(string email)
        {
            return db.Users.FirstOrDefault(u => u.Email == email);
        }

        public User? FindByIdAsync(int id)
        {
            return db.Users.Find(id);
        }

        public string GenerateEmailConfirmationTokenAsync(int userId)
        {
            //email token
            var token = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(token);
            }

            var user = db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                throw new ArgumentException("Invalid user id");
            }
            user.ConfirmCode = Convert.ToBase64String(token);
            db.SaveChanges();
            return Convert.ToBase64String(token);
        }

    }
}
