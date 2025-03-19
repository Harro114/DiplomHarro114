namespace Diplom.Models;

public class ExpUsers
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int ExpValue  { get; set; }
    
    public Accounts Account { get; set; }
    
    public ExpUsers ExpUser { get; set; }
}