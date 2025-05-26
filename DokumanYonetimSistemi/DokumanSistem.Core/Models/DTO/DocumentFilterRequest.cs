using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokumanSistem.Core.Models.DTO
{
    public class DocumentFilterRequest
    {
        public string CategoryGuid { get; set; } = string.Empty;
         public Dictionary<string, string>? Filters { get; set; }
    }

}
