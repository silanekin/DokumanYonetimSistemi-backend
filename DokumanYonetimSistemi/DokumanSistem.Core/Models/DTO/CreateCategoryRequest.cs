using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokumanSistem.Core.Models.DTO
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }  
        public bool IsGroup { get; set; } 
        public string? IndexData { get; set; } 
    }

}
