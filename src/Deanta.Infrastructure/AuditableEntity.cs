using System;

namespace Deanta.Infrastructure
{
    /// <summary>
    /// Defines an AuditableEntity
    /// </summary>
    public abstract class AuditableEntity : StatusEntity, IAuditableEntity
    {
        public DateTime? CreatedAt { get; protected set; }

        public string CreatedBy { get; protected set; }

        public DateTime? UpdatedAt { get; protected set; }

        public string UpdatedBy { get; protected set; }

        public void SetUpdated(DateTime time, string userId)
        {
            UpdatedAt = time;
            UpdatedBy = userId;
        }

        public virtual void SetCreated(DateTime time, string userId)
        {
            CreatedAt = time;
            CreatedBy = userId;
        }
    }
}
