namespace Domain.Entities;

public class Conversation : BaseEntity
{
    public required List<Guid> Participants { get; set; }
    public required ConversationType Type { get; set; }
    public GroupCreationInfo Group { get; set; }
    public LastMessageInfo LastMessage { get; set; }
    public List<Guid> SeenBy { get; set; }
    //TODO: Implement a variable for counting unread messages
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum ConversationType
{
    direct, 
    group
}


public class GroupCreationInfo
{
    public string Name { get; set; }
    public Guid CreatedBy { get; set; }
}

public class LastMessageInfo
{
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid Sender { get; set; }
}