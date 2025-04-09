using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Models;

public class ExchangeDiscounts
{
    public int DiscountId { get; set; }
    public Discounts Discount { get; set; }
    public int DiscountExchangeOneId { get; set; }
    public Discounts DiscountOne { get; set; }
    public int DiscountExchangeTwoId { get; set; }
    public Discounts DiscountTwo { get; set; }
}