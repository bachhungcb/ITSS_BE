namespace Domain.Entities;

public class Message : BaseEntity
{
    //Sender Foreignkey
    public required Guid SenderId {get; set;}
    public User Sender { get; set; }
    
    //Conversation Foreign Key
    public required Guid ConversationId {get; set;}
    public Conversation Conversation {get; set;}
    
    public string Content {get; set;}
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    

}