using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApp.Models
{
    public class Calculation
    {
        public int Id { get; set; }
        public string NameA { get; set; }
        public string DoBA { get; set; }
        public string FavoriteA { get; set; }
        public string NameB { get; set; }
        public string DoBB { get; set; }
        public string FavoriteB { get; set; }
        public string Agent { get; set; }
        public bool IsCalculated { get; set; }
        public int Result { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
