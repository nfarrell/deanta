using System;

namespace Deanta.Web
{
    public interface IAuditableEntity
    {
        DateTime? CreatedAt { get; }

        string CreatedBy { get; }

        DateTime? UpdatedAt { get; }

        string UpdatedBy { get; }

        void SetUpdated(DateTime time, string username);

        void SetCreated(DateTime time, string username);
    }
}
