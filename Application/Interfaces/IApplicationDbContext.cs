using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Conversation> Conversation { get; set; }
    DbSet<Message> Messages { get; set; }
    DbSet<Friend> Friends { get; set; }
    DbSet<FriendRequest> FriendRequests { get; set; }
    Task<int> SaveChanges();
}