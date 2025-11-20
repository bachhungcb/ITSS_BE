namespace Domain.Entities;

public class User : BaseEntity
{
    public required string FullName { get; set; }
    public required string UserName { get; set; } //Display name
    public required string PasswordHash { get; set; }
    public string Email { get; set; } //Unique
    public string AvatarUrl { get; set; }
    public string Bio { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // THÊM VÀO: Navigation Properties cho FriendRequest
    public ICollection<FriendRequest> SentFriendRequests { get; set; }
    public ICollection<FriendRequest> ReceivedFriendRequests { get; set; }
    
    //Navigation properties cho Friend
    // Danh sách những người mà User này đã kết bạn (User này là UserA)
    public ICollection<Friend> Friendships { get; set; }
    
    // Danh sách những người đã kết bạn với User này (User này là UserB)
    public ICollection<Friend> FriendedBy { get; set; }
    
    //For password reset
    public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
    
}