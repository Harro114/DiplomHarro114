namespace Diplom.Models;

public class Accounts
{
    public int AccountId { get; set; }
    public string Username { get; set; }
    public string UserLastName { get; set; }
    public string UserFirstName { get; set; }
    public bool Sex { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ExpUsers ExpUser { get; set; }

}