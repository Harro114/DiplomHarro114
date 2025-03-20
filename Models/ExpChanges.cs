namespace Diplom.Models;

public class ExpChanges
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int ExpUserId { get; set; }
    public int Value { get; set; }
    public int CurrentBalance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    
    // Ссылка на объект Account
    public Accounts Account { get; set; }

    // Новое свойство для ExpUser
    public ExpUsers ExpUser { get; set; }

}