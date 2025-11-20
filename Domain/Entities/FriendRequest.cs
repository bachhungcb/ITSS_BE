namespace Domain.Entities;

public class FriendRequest : BaseEntity
{
    //Foreign Key for sender
    public required Guid SenderId { get;set; }
    public User Sender { get;set; }
    
    //Receiver Foreign Key
    public required Guid ReceiverId { get;set; }
    public User Receiver { get;set; }
    
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}