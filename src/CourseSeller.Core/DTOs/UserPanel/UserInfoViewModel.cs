namespace CourseSeller.Core.DTOs.UserPanel
{
    public class UserInfoViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime RegisterDateTime { get; set; }
        public int Wallet { get; set; }
        public SideBarViewModel SideBarViewModel { get; set; }
    }
}