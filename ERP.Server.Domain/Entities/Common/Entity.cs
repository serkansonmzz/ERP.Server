using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;

namespace ERP.Server.Domain.Entities.Common;

public abstract class Entity
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
    public DateTime CreatedAt { get; protected set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime? UpdatedAt { get; protected set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime? DeletedAt { get; protected set; }
    
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    public string? DeletedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }

    protected void SetUpdated(string? updatedBy = null)
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

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
