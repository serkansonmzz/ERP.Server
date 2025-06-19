
namespace ERP.Server.Domain.Entities.Common
{
    public interface ISoftDelete
    {
        public bool IsDeleted { get; }
        DateTime? DeletedAt { get; }
        public string? DeletedBy { get; set; }
    }
}