using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom.Models;

public class Discounts
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public bool isActive { get; set; }
    public int DicountSize { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<ProductsStore>? ProductsId { get; set; }
    public ProductsStore? ProductsStore { get; set; }
    public List<CategoriesStore>? CategoriesId { get; set; }
    public CategoriesStore?CategoriesStore { get; set; }
    public int Amount { get; set; }
    public bool isPrimary {get;set;}

}