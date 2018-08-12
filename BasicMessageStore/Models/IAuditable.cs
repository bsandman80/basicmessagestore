using System;
using BasicMessageStore.Models.Users;

namespace BasicMessageStore.Models
{
    /// <summary>
    /// Generic interface for auditable columns
    /// </summary>
    public interface IAuditable
    {
        User CreatedBy { get; set; }
        DateTime Created { get; set; }
        DateTime? Updated { get; set; }
    }
}