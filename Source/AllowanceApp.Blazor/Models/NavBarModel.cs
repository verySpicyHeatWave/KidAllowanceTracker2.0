
namespace AllowanceApp.Blazor.Models
{
    public class NavBarModel(AccountViewModel account)
    {
        public string Name = account.Name;
        public int ID = account.ID;
    }
}
