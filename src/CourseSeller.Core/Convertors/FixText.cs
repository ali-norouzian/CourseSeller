namespace CourseSeller.Core.Convertors
{
    public static class FixText
    {
        public static string FixEmail(string email)
        {
            return email.Trim().ToLower();
        }
    }
}