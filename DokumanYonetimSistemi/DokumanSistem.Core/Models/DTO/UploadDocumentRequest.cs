using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokumanSistem.Core.Models.DTO
{
    public class UploadDocumentRequest
    {
        public Guid CategoryId { get; set; }    
        public IFormFile Document { get; set; }  
        public IndexField IndexFields { get; set; }  
        public Guid CategoryGuid { get; set; }
    }
}
