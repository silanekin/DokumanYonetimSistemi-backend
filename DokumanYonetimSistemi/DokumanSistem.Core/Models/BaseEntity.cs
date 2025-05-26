using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokumanSistem.Core.Models
{
    public class BaseEntity<T>
    {
        public T Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; }
    }
}

