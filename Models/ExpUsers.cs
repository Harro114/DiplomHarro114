using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom.Models;

public class ExpUsers
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    public Accounts Accounts { get; set; }
    public int ExpValue  { get; set; }

}