using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
using ERP.Server.Domain.Entities.Common;

namespace ERP.Server.Domain.Entities.Common;

public abstract class Entity: ISoftDelete
{
    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [BsonId]
    public Guid Id { get; protected set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime? UpdatedAt { get; protected set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime? DeletedAt { get; protected set; }
    
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; protected set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; protected set; }

    public void SetUpdated(string? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(updatedBy))
            UpdatedBy = updatedBy;
    }

    public void SetCreatedBy(string createdBy)
    {
        if (!string.IsNullOrEmpty(createdBy))
            CreatedBy = createdBy;
    }

    public virtual void Delete(string? deletedBy = null)
    {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
            SetUpdated(deletedBy);
    }
}
