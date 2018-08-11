namespace BasicMessageStore.Client
{
    public class CommandLineParser
    {
        private string ApiUri { get; }
        
        public CommandLineParser(string apiUri)
        {
            ApiUri = apiUri;
        } 
        
        public bool Parse(string commandLine, out ApiCommand command)
        {
            
        }
    }
}