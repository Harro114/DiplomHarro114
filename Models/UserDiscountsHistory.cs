using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Diplom.Models;

public class UserDiscountsHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    public Accounts Accounts { get; set; }
    public int DiscountId { get; set; }
    [ForeignKey("DiscountId")]
    public Discounts Discounts { get; set; }
    public DateTime DateAccruals { get; set; }
    public DateTime DateDelete { get; set; } = DateTime.Now;
}