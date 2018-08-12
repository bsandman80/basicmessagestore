using System;

namespace BasicMessageStore.Exceptions
{
    public class MessageStoreException : Exception
    {
        public int ErrorCode { get; set; }
        
        public MessageStoreException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}