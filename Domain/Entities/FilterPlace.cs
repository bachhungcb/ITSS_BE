namespace Domain.Entities;
//TO DO
public class FilterPlace : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}