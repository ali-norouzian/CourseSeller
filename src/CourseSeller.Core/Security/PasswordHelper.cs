namespace CourseSeller.Core.Security
{
    public static class PasswordHelper
    {
        // hash password with bcryot hash algorithm
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 11);
        }

        // verify password with bcryot hash algorithm
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}