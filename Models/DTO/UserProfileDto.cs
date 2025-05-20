namespace Gamification.Models.DTO;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public bool isBlocked { get; set; }

}