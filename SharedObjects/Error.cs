using System;

namespace SharedObjects
{
    
    public class Error
    {
        
        public string Text { get; set; }
        public DateTime Timestamp { get; private set; } = DateTime.Now;
    }
}
