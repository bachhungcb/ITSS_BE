namespace Domain.Entities;

public class Friend : BaseEntity
{
    public required Guid UserA { get; set; }
    public User FriendA { get; set; }
    
    public required Guid UserB { get; set; }
    public User FriendB { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}