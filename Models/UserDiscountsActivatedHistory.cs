using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gamification.Models;

public class UserDiscountsActivatedHistory
{
    [Key]
    public int Id { get; set; }
    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    public Accounts Accounts { get; set; }
    public string? DiscountName { get; set; }
    public int DiscountId { get; set; }
    
    [ForeignKey("DiscountId")]
    public Discounts Discounts { get; set; }
    public List<ProductsStore>? ProductsId { get; set; }
    
    public List<CategoriesStore>? CategoriesId { get; set; }
    public DateTime DateActivateDiscount { get; set; }
    public DateTime DateDelete { get; set; } = DateTime.UtcNow;
}