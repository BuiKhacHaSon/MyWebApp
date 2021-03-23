using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BodyMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string Agent { get; set; }
        public string Platform { get; set; }
        public string Browser { get; set; }
        public string Device { get; set; }
    }
}
