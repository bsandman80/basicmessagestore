using System;
using System.ComponentModel.DataAnnotations;
using BasicMessageStore.Models.Security;

namespace BasicMessageStore.Models.Messages
{
    public class Message : Model, IAuditable
    {
        public User.User CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        
        [Required]
        [MaxLength(128)]
        public string Header { get; set; }
        
        [Required]
        [MaxLength(1028)]
        public string Body { get; set; }
    }
}