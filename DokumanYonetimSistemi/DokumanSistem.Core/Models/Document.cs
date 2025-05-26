using System.ComponentModel.DataAnnotations;

namespace DokumanSistem.Core.Models
{
    public class Document : BaseEntity<Guid>
    {
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileData { get; set; }

        public Guid CategoryGuid { get; set; }
        public string? Metadata { get; set; }

        public Category? Category { get; set; }

        public ICollection<DocumentIndex> DocumentIndexes { get; set; } = new List<DocumentIndex>();

    }
}
