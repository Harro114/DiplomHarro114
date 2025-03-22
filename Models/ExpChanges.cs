using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom.Models;

public class ExpChanges
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    public Accounts Accounts { get; set; }
    public int ExpUserId { get; set; }
    [ForeignKey("ExpUserId")]
    public ExpUsers ExpUsers { get; set; }
    public int Value { get; set; }
    public int CurrentBalance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

}