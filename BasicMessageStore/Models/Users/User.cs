using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace BasicMessageStore.Models.Users
{
    public class User : Model 
    {     
        public string Username { get; set; }
        
        /// <summary>
        /// Property is used to bind posted data from user
        /// </summary>
        [NotMapped]
        public string Password { get; set; }
                
        /// <summary>
        /// Do not expose the password in responses
        /// </summary>
        [JsonIgnore]
        public string HashedPassword { get; set; }
    }
}