namespace Cortex.helper
{
    public class Helper
    {
        public static string IncreptPassword(string pass)
        {
            string password = pass!;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }
    }
}
