using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BasicMessageStore.Models.Users
{
    public class User : Model 
    {     
        public string Username { get; set; }    
        
        /// <summary>
        /// Do not expose the password in responses
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; }
    }
}