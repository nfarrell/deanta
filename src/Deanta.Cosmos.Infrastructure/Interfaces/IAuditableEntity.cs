using System;

namespace Deanta.Cosmos.Infrastructure.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime? CreatedAt { get; }

        string CreatedBy { get; }

        DateTime? UpdatedAt { get; }

        string UpdatedBy { get; }

        /// <summary>
        /// Sets the updated.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        void SetUpdated(DateTime time, string username);

        /// <summary>
        /// Sets the created.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        void SetCreated(DateTime time, string username);
    }
}
