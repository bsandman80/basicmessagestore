using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BasicMessageStore.Models.User
{
    public class User : Model 
    {
        [Required]
        [MinLength(5)]      
        public string Username { get; set; }    
        
        [MinLength(8)]
        [JsonIgnore]
        public string Password { get; set; }
    }
}