﻿namespace Gamification.Models.DTO;

public class GetUserDiscountsActivatedHistoryDTO
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string UserLastName { get; set; }
    public string UserFirstName { get; set; }
    public string DiscountName { get; set; }
    public DateTime DateActivateDiscount { get; set; }
    public DateTime DateDelete { get; set; }
}