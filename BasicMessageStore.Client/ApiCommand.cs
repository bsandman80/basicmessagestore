using System.Net.Http;

namespace BasicMessageStore.Client
{
    public class ApiCommand
    {
        private HttpClient _client { get; set; }
        public ApiCommand()
        {
            _client = new HttpClient();
        }
        
        public string Execute() 
        {
            
        }
    }
}