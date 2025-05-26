using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokumanSistem.Core.Models
{
    public class DocumentIndex
    {
        public int Id { get; set; }
        public Guid DocumentId { get; set; }
        public required string? CompanyName { get; set; }
        public DateTime? DocumentDate { get; set; }
        public required string? AdditionalInfo { get; set; }


        public Document Document { get; set; }
    }

}
