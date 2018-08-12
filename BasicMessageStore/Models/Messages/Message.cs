using System;
using BasicMessageStore.Models.Users;

namespace BasicMessageStore.Models.Messages
{
    public class Message : Model, IAuditable
    {
        public User CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        
        public string Header { get; set; }
        
        public string Body { get; set; }
    }
}