namespace Sisprenic.Domain.Entities;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedOn { get; set; }
}
