namespace CourseSeller.Core.Security
{
    public interface IPasswordHelper
    {
        Task<string> HashPassword(string password);
        Task<bool> VerifyPassword(string password, string hashedPassword);
    }

    public class PasswordHelper:IPasswordHelper
    {
        // hash password with bcryot hash algorithm
        public async Task<string> HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 11);
        }

        // verify password with bcryot hash algorithm
        public async Task<bool> VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

    }
}