using System;
using BasicMessageStore.Models.Security;

namespace BasicMessageStore.Models
{
    public interface IAuditable
    {
        User.User CreatedBy { get; set; }
        DateTime Created { get; set; }
        DateTime? Updated { get; set; }
    }
}